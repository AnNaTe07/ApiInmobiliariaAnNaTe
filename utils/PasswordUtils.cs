using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ApiInmobiliariaAnNaTe.utils
{
    public static class PasswordUtils
    {
        // Hashea una contraseña usando un sal generado, recibe una contraseña y devuelve una tupla que contiene la contraseña hasheada y el sal utilizado
        public static (string hashedPassword, string salt) HashPassword(string password)
        {
            // Generar un sal
            var saltBytes = new byte[16];//Se genera un sal aleatorio de 16 bytes
            using (var rng = new RNGCryptoServiceProvider())
            {// Utiliza un generador de números aleatorios criptográficamente seguro para llenar el arreglo de bytes
                rng.GetBytes(saltBytes);
            }
            //Convierte el sal a una cadena en Base64 para que sea más fácil de almacenar
            var salt = Convert.ToBase64String(saltBytes);

            // Hashear la contraseña con el sal
            var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(//usa el algoritmo PBKDF2 para hashear la contraseña
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,//Número de iteraciones para hacer el hashing más lento y así aumentar la seguridad
                numBytesRequested: 256 / 8));//tamaño del hash resultante en bytes, 256/8 = 32

            return (hashedPassword, salt);//retorn la contraseña hasheada y el sal en forma de tupla
        }

        // Verifica una contraseña dada y su hash
        public static bool VerifyPassword(string password, string hashedPassword, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);//Convierte el sal desde Base64 a un arreglo de bytes para su uso en el hashing
            var hashToVerify = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            //Realiza el hashing de la contraseña ingresada con el mismo sal y configuración que se utilizó al generar el hash original

            return hashToVerify == hashedPassword;//Compara el hash generado con el hash almacenado y devuelve true si son iguales
        }
    }

}

// prf: KeyDerivationPrf.HMACSHA256 :algoritmo de función de pseudorandom (PRF) HMAC (Hash-based Message Authentication Code) con SHA-256 como el hash.
//HMAC: mecanismo que combina una función hash (en este caso, SHA-256) con una clave secreta para proporcionar autenticación y asegurar la integridad de los datos.
//SHA-256: Es un algoritmo de hash que produce un hash de 256 bits. Es parte de la familia SHA-2 y es  utilizado por su resistencia a colisiones y seguridad.
//Al usar HMAC con SHA-256 como PRF, se asegura que el proceso de derivación de claves sea seguro y resistente a ataques, ya que cualquier cambio en la entrada (como la contraseña o el sal) resultará en un hash completamente diferente