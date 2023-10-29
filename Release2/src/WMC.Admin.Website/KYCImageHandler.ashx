<%@ WebHandler Language="C#" Class="KYCImageHandler" %>

using System;
using System.Data;
using System.IO;
using System.Net;
using System.Web;
using System.Data.SqlClient;
using System.Drawing;

public class KYCImageHandler : IHttpHandler
{
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "image/jpg";
        byte[] image = null;
        string ImageId = context.Request.QueryString["ImageId"];
        string backup = context.Request.QueryString["backup"];
        try
        {
            if (!string.IsNullOrEmpty(backup) && backup.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                if (TryGetOldDocument(ImageId, out image))
                {
                    context.Response.BinaryWrite(image);
                    context.Response.End();
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }

                return;
            }

            bool? flipped = null;
            int? angle = null;
            bool isEdit = false;
            string uniqueFilename = "";
            string originalFilename = "";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LocalConnectionString"].ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT NULL as [File], [Id],[Type],[Note],[UniqueFilename],[OriginalFilename] FROM KycFile WHERE (Id = @Id)", conn);
                cmd.Parameters.Add(new SqlParameter("@Id", ImageId));
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    dr.Read();
                    if (dr.HasRows)
                    {
                        uniqueFilename = dr["UniqueFilename"].ToString();
                        image = WMC.Logic.KYCFileHandler.GetFile(uniqueFilename);
                        originalFilename = dr["OriginalFilename"].ToString();
                        object ext = "";
                        if (!originalFilename.Contains(".") && originalFilename.Contains("FaceTec"))
                        {
                            ext = "jpg";
                        }
                        else
                        {
                            ext = originalFilename.Split('.').GetValue(1);
                        }
                        if (ext.ToString() == "pdf")
                        {
                            context.Response.ContentType = "application/pdf";
                        }
                        string edit = context.Request.QueryString["edit"];
                        if (!string.IsNullOrEmpty(edit))
                        {
                            // Edit Image
                            bool _flipped;
                            int _angle;
                            if (!string.IsNullOrEmpty(context.Request.QueryString["flipped"]) && bool.TryParse(context.Request.QueryString["flipped"], out _flipped))
                            {
                                flipped = _flipped;
                                isEdit = true;
                            }

                            if (!string.IsNullOrEmpty(context.Request.QueryString["angle"]) && int.TryParse(context.Request.QueryString["angle"], out _angle))
                            {
                                angle = _angle;
                                isEdit = true;
                            }
                        }
                        else
                        {
                            // Edit Image
                            if (image == null || image.Length == 0)
                            {
                                if (TryGetOldDocument(ImageId, out image))
                                {
                                    context.Response.BinaryWrite(image);
                                    TryUpdateDocument(ImageId, image);
                                    // context.Response.StatusCode = (int)HttpStatusCode.ResetContent;
                                }
                                else
                                {
                                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                    return;
                                }
                            }
                            else
                            {
                                context.Response.BinaryWrite(image);
                            }
                        }
                    }
                    else
                    {
                        if (TryGetOldDocument(ImageId, out image))
                        {
                            context.Response.BinaryWrite(image);
                            TryUpdateDocument(ImageId, image);
                            // context.Response.StatusCode = (int)HttpStatusCode.ResetContent;
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        }
                    }
                }
            }
            if (isEdit)
            {
                long userId = 0;
                if (context.Session != null)
                {
                    if (!(context.Session["UserId"] != null &&
                          context.Session["UserRole"] != null &&
                          long.TryParse(context.Session["UserId"].ToString(), out userId) &&
                          context.Session["UserRole"].ToString().Equals("Admin", StringComparison.InvariantCultureIgnoreCase)))
                    {
                        context.Response.StatusCode = 403;
                        context.Response.End();
                        return;
                    }
                }

                TransformImageAsync(ImageId, image, flipped, angle, uniqueFilename, originalFilename);
            }

        }
        catch (Exception ex)
        {
            string fileName1 = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\KycFiles\\" + string.Format("{0}-{1:yyMMddHHmmss}_log.txt", ImageId, DateTime.Now);
            File.WriteAllText(fileName1, string.Format(" {3:yyMMddHHmmss} : {0} - {1}. \n Stack Trace: {2}", ImageId, ex.Message, ex.StackTrace, DateTime.Now));
        }

        context.Response.End();
    }

    public static bool TransformImageAsync(string ImageId, byte[] image, bool? flipped, int? angle, string uniqueFilename, string originalFilename)
    {
        if (string.IsNullOrEmpty(ImageId) || ImageId.Length <= 0)
            return false;
        if (!flipped.HasValue && !angle.HasValue)
            return false;
        if (image == null || image.Length == 0)
        {
            byte[] image1;
            if (TryGetOldDocument(ImageId, out image1))
            {
                image = image1;
            }
            else
            {
                return false;
            }
        }

        string fileName1 = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\KycFiles\\" + string.Format("{0}-a_{1}-f_{2}-d_{5:yyMMddHHmmss}-u_{3}-o_{4}", ImageId, angle, flipped, uniqueFilename, originalFilename, DateTime.Now);
        // try
        // {
        File.WriteAllBytes(fileName1, image);
        // }
        // catch { }

        using (MemoryStream ms = new MemoryStream(image))
        {
            Bitmap bmp = new Bitmap(ms);
            Bitmap newBmp = RotateImg(bmp, angle.GetValueOrDefault(0), flipped.GetValueOrDefault());
            // bmp.Dispose();
            if (newBmp != null)
            {
                ImageConverter converter = new ImageConverter();
                byte[] byteArray = (byte[])converter.ConvertTo(newBmp, typeof(byte[]));
                if (byteArray != null || byteArray.Length > 0)
                {
                    TryUpdateDocument(ImageId, byteArray);
                }

                newBmp.Dispose();
            }
        }
        return true;
    }

    private static Bitmap RotateImg(Bitmap bmp, float angle, bool flip = false)
    {
        if (angle != 0)
        {
            if (angle == 90 || angle == -270)
            {
                bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            else if (angle == 180 || angle == -180)
            {
                bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }
            else if (angle == 270 || angle == -90)
            {
                bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
        }

        if (flip)
        {
            bmp.RotateFlip(RotateFlipType.Rotate180FlipY);
        }

        return bmp;

        // // can be from param
        // Color bkColor = Color.White;
        // 
        // angle = angle % 360;
        // if (angle > 180)
        //     angle -= 360;
        // 
        // System.Drawing.Imaging.PixelFormat pf = default(System.Drawing.Imaging.PixelFormat);
        // if (bkColor == Color.Transparent)
        // {
        //     pf = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
        // }
        // else
        // {
        //     pf = bmp.PixelFormat;
        // }
        // 
        // float sin = (float)Math.Abs(Math.Sin(angle * Math.PI / 180.0)); // this function takes radians
        // float cos = (float)Math.Abs(Math.Cos(angle * Math.PI / 180.0)); // this one too
        // float newImgWidth = sin * bmp.Height + cos * bmp.Width;
        // float newImgHeight = sin * bmp.Width + cos * bmp.Height;
        // float originX = 0f;
        // float originY = 0f;
        // 
        // if (angle > 0)
        // {
        //     if (angle <= 90)
        //         originX = sin * bmp.Height;
        //     else
        //     {
        //         originX = newImgWidth;
        //         originY = newImgHeight - sin * bmp.Width;
        //     }
        // }
        // else
        // {
        //     if (angle >= -90)
        //         originY = sin * bmp.Width;
        //     else
        //     {
        //         originX = newImgWidth - sin * bmp.Height;
        //         originY = newImgHeight;
        //     }
        // }
        // 
        // Bitmap newImg = new Bitmap((int)newImgWidth, (int)newImgHeight, pf);
        // Graphics g = Graphics.FromImage(newImg);
        // g.Clear(bkColor);
        // g.TranslateTransform(originX, originY); // offset the origin to our calculated values
        // g.RotateTransform(angle); // set up rotate
        // g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
        // g.DrawImageUnscaled(bmp, 0, 0); // draw the image at 0, 0
        // g.Dispose();
        // 
        // if (flip)
        // {
        //     newImg.RotateFlip(RotateFlipType.Rotate180FlipY);
        // }
        // 
        // return newImg;
    }

    private static bool TryUpdateDocument(string ImageId, byte[] byteArray)
    {
        using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["LocalConnectionString"].ConnectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("UPDATE KycFile SET [File] = @File WHERE (Id = @Id)", conn);
            cmd.Parameters.Add("@Id", SqlDbType.Int).Value = ImageId;
            cmd.Parameters.Add("@File", SqlDbType.Binary).Value = byteArray;
            var result = cmd.ExecuteNonQuery();
            return result > 0;
        }
    }

    private static bool TryGetOldDocument(string ImageId, out byte[] imageByteArray)
    {
        string oldFileName = GetFileOldFile(ImageId);
        if (!string.IsNullOrEmpty(oldFileName))
        {
            imageByteArray = File.ReadAllBytes(oldFileName);
            return true;
        }
        else
        {
            imageByteArray = null;
            return false;
        }
    }

    private static string GetFileOldFile(string id)
    {
        string path = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\KycFiles\\";
        string[] files = Directory.GetFiles(path, id + "-a_*");
        if (files != null && files.Length > 0)
        {
            DateTime created = DateTime.MinValue;
            string oldest = null;
            foreach (var file in files)
            {
                var finf = new FileInfo(file);
                if (created < finf.CreationTime)
                {
                    created = finf.CreationTime;
                    oldest = file;
                }
            }

            return oldest;
        }
        else
        {
            return null;
        }
        // string fileName1 = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\KycFiles\\" + string.Format("{0}-a_{1}-f_{2}-d_{5:yyMMddHHmmss}-u_{3}-o_{4}", ImageId, angle, flipped, uniqueFilename, originalFilename, DateTime.Now);
    }
}