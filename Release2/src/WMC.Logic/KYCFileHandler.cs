using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMC.Data;

namespace WMC.Logic
{
    public class KYCFileHandler
    {
        public static KycFile AddNewKYC(Stream inputStream, string phoneNumber, string fileName, string kycType, string sessionID = "NULL", int faceTecStatus = 0)
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            var kycFileTypeId = DataUnitOfWork.KycTypes.Get(q => q.Text == kycType).Select(x => x.Id).FirstOrDefault();
            var userId = DataUnitOfWork.Users.Get(q => q.Phone == phoneNumber).Select(x => x.Id).FirstOrDefault();
            return AddNewKYCFile(inputStream, userId, fileName, kycFileTypeId, sessionID, faceTecStatus);
        }

        public static KycFile AddNewKYCFile(Stream inputStream, long userid, string fileName, long kycTypeId, string sessionID = "NULL", int faceTecStatus = 0)
        {
            var DataUnitOfWork = new DataUnitOfWork(new RepositoryProvider(new RepositoryFactories()));
            var KYCFileLocation = SettingsManager.GetDefault().Get("KYCFileLocation").Value;
            if (KYCFileLocation == null)
                throw new Exception("Unable to find 'KYCFileLocation' key in AppSettings.");

            var kycFolder = KYCFileLocation;
            if (!Directory.Exists(kycFolder))
            {
                Directory.CreateDirectory(kycFolder);
            }

            //var kycFileType = DataUnitOfWork.KycTypes.Get(q => q.Id == kycTypeId).FirstOrDefault();
            //var user = DataUnitOfWork.Users.Get(q => q.Id == userid).FirstOrDefault();
            var newKycFileName = Path.GetFileNameWithoutExtension((Path.GetRandomFileName()));
            var kycFilePath = Path.Combine(kycFolder, newKycFileName);

            var fileByteArray = StreamToByteArray(inputStream);
            System.IO.File.WriteAllBytes(kycFilePath, fileByteArray);
            //add new uploaded file to KYCFile table
            var file = new KycFile
            {
                OriginalFilename = fileName,
                UniqueFilename = newKycFileName,
                //File = fileByteArray,
                Type = kycTypeId,
                Uploaded = DateTime.Now,
                UserId= userid,
                SessionId = sessionID,
                FaceTecStatus = faceTecStatus
            };

            DataUnitOfWork.KycFiles.Add(file);
            DataUnitOfWork.Commit();
            
            return file;
        }

        /// <summary>
        /// method to convert stream to byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static byte[] StreamToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }

        public static string GetFilePath(string uniqueFilename)      
        {
            var KYCFileLocation = SettingsManager.GetDefault().Get("KYCFileLocation").Value;
            if (KYCFileLocation == null)
                throw new Exception("Unable to find 'KYCFileLocation' key in AppSettings.");
            var kycFolder = KYCFileLocation;
            return Path.Combine(kycFolder, uniqueFilename);
        }

        public static byte[] GetFile(string uniqueFilename)
        {
            var KycFileName = GetFilePath(uniqueFilename);
            byte[] buffer = File.ReadAllBytes(KycFileName);
            return buffer;
        }
    }
}
