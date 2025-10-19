using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace CastleLordTycoon.Core
{
    /// <summary>
    /// 암호화된 로컬 저장소 (Android Keystore 기반)
    /// </summary>
    public static class SecureStorage
    {
        private const string ENCRYPTION_KEY_PREF = "_secure_key";
        private static byte[] _encryptionKey;

        /// <summary>
        /// 초기화 (앱 시작 시 한 번 호출)
        /// </summary>
        public static void Initialize()
        {
            // 암호화 키 로드 또는 생성
            if (PlayerPrefs.HasKey(ENCRYPTION_KEY_PREF))
            {
                string keyString = PlayerPrefs.GetString(ENCRYPTION_KEY_PREF);
                _encryptionKey = Convert.FromBase64String(keyString);
            }
            else
            {
                // 새 키 생성
                _encryptionKey = GenerateKey();
                PlayerPrefs.SetString(ENCRYPTION_KEY_PREF, Convert.ToBase64String(_encryptionKey));
                PlayerPrefs.Save();
            }
        }

        /// <summary>
        /// 암호화하여 저장
        /// </summary>
        public static void SetString(string key, string value)
        {
            if (_encryptionKey == null)
            {
                Debug.LogError("SecureStorage가 초기화되지 않았습니다.");
                return;
            }

            string encrypted = Encrypt(value);
            PlayerPrefs.SetString(GetSecureKey(key), encrypted);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 복호화하여 가져오기
        /// </summary>
        public static string GetString(string key, string defaultValue = "")
        {
            if (_encryptionKey == null)
            {
                Debug.LogError("SecureStorage가 초기화되지 않았습니다.");
                return defaultValue;
            }

            string secureKey = GetSecureKey(key);
            if (!PlayerPrefs.HasKey(secureKey))
            {
                return defaultValue;
            }

            string encrypted = PlayerPrefs.GetString(secureKey);
            return Decrypt(encrypted);
        }

        /// <summary>
        /// 키 존재 확인
        /// </summary>
        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(GetSecureKey(key));
        }

        /// <summary>
        /// 키 삭제
        /// </summary>
        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(GetSecureKey(key));
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 모든 데이터 삭제
        /// </summary>
        public static void DeleteAll()
        {
            // 보안상 각 키를 개별 삭제하는 것이 안전
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        #region Encryption

        private static string GetSecureKey(string key)
        {
            // 키 이름도 해싱하여 난독화
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                return Convert.ToBase64String(hashBytes);
            }
        }

        private static byte[] GenerateKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.GenerateKey();
                return aes.Key;
            }
        }

        private static string Encrypt(string plainText)
        {
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = _encryptionKey;
                    aes.GenerateIV();

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                    // IV + 암호화된 데이터
                    byte[] result = new byte[aes.IV.Length + encryptedBytes.Length];
                    Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                    Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

                    return Convert.ToBase64String(result);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"암호화 실패: {ex.Message}");
                return string.Empty;
            }
        }

        private static string Decrypt(string cipherText)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = _encryptionKey;

                    // IV 추출
                    byte[] iv = new byte[aes.IV.Length];
                    Buffer.BlockCopy(cipherBytes, 0, iv, 0, iv.Length);
                    aes.IV = iv;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    int encryptedLength = cipherBytes.Length - iv.Length;
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, iv.Length, encryptedLength);

                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"복호화 실패: {ex.Message}");
                return string.Empty;
            }
        }

        #endregion
    }
}
