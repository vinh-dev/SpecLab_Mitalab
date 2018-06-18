using System.IO;
using System.Security.Cryptography;
using SpecLab.Business.BusinessEnum;
using SpecLab.Business.BusinessObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Spring.Context.Support;

namespace SpecLab.Business
{
    public static class CommonUtils
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }

            return value.ToString();
        }

        public static T GetEnumFromValue<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public static T GetBusinessObject<T>()
        {
            var listItem = ContextRegistry.GetContext().GetObjectsOfType(typeof(T)).Values;
            return listItem.Cast<T>().FirstOrDefault();
        }

        public static T GetBusinessObject<T>(string objectName)
        {
            return (T)ContextRegistry.GetContext().GetObject(objectName);
        }

        public static List<UserRightCode> GetListDefaultUserRightCode()
        {
            List<UserRightCode> listDefault = new List<UserRightCode>(new UserRightCode[]
            {
                UserRightCode.R00100,
                UserRightCode.R00101
            });

            return listDefault;
        }

        public static string EncryptData(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(CommonConstant.AppName, Encoding.ASCII.GetBytes(CommonConstant.ApplicationId)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(CommonConstant.CryptoVIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(CommonConstant.AppName, Encoding.ASCII.GetBytes(CommonConstant.ApplicationId)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(CommonConstant.CryptoVIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

        public static string GetErrorMessage(ErrorCode code, params object[] inputParams)
        {
            string errorMessage = String.Format(GetEnumDescription(code), inputParams);
            return errorMessage;
        }

        public static string GetErrorMessage(BusinessException businessException)
        {
            string errorMessage = string.Empty;
            if (businessException.ErrorParams != null)
            {
                errorMessage = String.Format(GetEnumDescription(businessException.ErrorCode), businessException.ErrorParams);
            }
            else
            {
                errorMessage = String.Format(GetEnumDescription(businessException.ErrorCode));
            }

            return errorMessage;
        }

        public static string GetStorageDisplay(string storageId, int locationNum)
        {
            const string StorageDisplayTemplate = "{0} - {1}";
            return string.Format(StorageDisplayTemplate, storageId, locationNum);
        }

        public static CalculatedCondition GetCalculatedCondition(TubeSampleStatus status, TubeSampleType type, double volume, int numberExport)
        {
            if (type== TubeSampleType.InStorage)
            {
                switch (status)
                {
                    case TubeSampleStatus.Good:
                        if (numberExport <= 0 && volume > 0)
                        {
                            return CalculatedCondition.InStorage;
                        }
                        if (volume <= 0)
                        {
                            return CalculatedCondition.OutOfVolume;// Hết
                        }
                        return CalculatedCondition.Remove;
                    case TubeSampleStatus.Corrupt:
                        return CalculatedCondition.Corrupt;
                    case TubeSampleStatus.Remove:
                        return CalculatedCondition.Remove;
                    case TubeSampleStatus.MoreTime:
                        return CalculatedCondition.MoreTime;
                    default:
                        return CalculatedCondition.Remove;
                }
            }
            else if (type== TubeSampleType.InUse)
            {
                if (status== TubeSampleStatus.Good)
                {
                    return CalculatedCondition.InUse;
                }
                else return CalculatedCondition.Remove;
            }
            else
            {
                return CalculatedCondition.Remove;
            }


        }

    }
}
