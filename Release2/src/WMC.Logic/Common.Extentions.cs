using System;
namespace WMC.Logic
{
    public static class CommonUUID
    {
        public static string UUID()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
