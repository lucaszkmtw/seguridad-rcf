using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace TGP.WSS.API.Helpers
{
    //Objetivo:
    //Hacer un cifrado Triple DES en un texto sin formato utilizando la clave proporcionada por el usuario. 
    //Se calcula un hash MD5 usando la clave proporcionada por el usuario. Y esa clave será usada para cifrar y descifrar el mensaje.
    //Explicación de DES:
    //DES es un algoritmo de cifrado de clave simétrica. La misma clave se utiliza para el cifrado y el descifrado. 
    //Por lo tanto en el uso de algoritmos de clave simétrica es necesario tener la misma clave para el descifrado que se utiliza para el cifrado. 
    //Pasos:
    //-Introducir alguna cadena como clave. 
    //-Calcular el MD5 hash de la clave para hacer la llave. 
    //-Utilizar esta llave para cifrar y descifrar el texto plano usando el Algoritmo TripleDES.

    public static class Encriptador
    {

        /// <summary>
        /// Metodo para Encriptar un texto.
        /// </summary>
        /// <param name="texto">Texto a encriptar</param>
        public static string Encriptar(string texto)
        {
            try
            {

                //string key = "qualityinfosolutions"; //llave para encriptar datos
                string key = "tesoreria2017llave"; //llave para encriptar datos

                byte[] keyArray;

                byte[] Arreglo_a_Cifrar = System.Text.UTF8Encoding.UTF8.GetBytes(texto);

                //Se utilizan las clases de encriptación MD5

                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();

                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

                hashmd5.Clear();

                //Algoritmo TripleDES
                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateEncryptor();

                byte[] ArrayResultado = cTransform.TransformFinalBlock(Arreglo_a_Cifrar, 0, Arreglo_a_Cifrar.Length);

                tdes.Clear();

                //se regresa el resultado en forma de una cadena
                texto = Convert.ToBase64String(ArrayResultado, 0, ArrayResultado.Length);

            }
            catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
            return texto;
        }


        /// <summary>
        /// Metodo para Desencriptar un texto.
        /// </summary>
        /// <param name="textoEncriptado">Texto a desencriptar</param>
        public static string Desencriptar(string textoEncriptado)
        {
            try
            {
                //string key = "qualityinfosolutions"; //llave para encriptar datos
                string key = "tesoreria2017llave";
                byte[] keyArray;
                byte[] Array_a_Descifrar = Convert.FromBase64String(textoEncriptado);

                //algoritmo MD5
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();

                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

                hashmd5.Clear();

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();

                byte[] resultArray = cTransform.TransformFinalBlock(Array_a_Descifrar, 0, Array_a_Descifrar.Length);

                tdes.Clear();
                textoEncriptado = UTF8Encoding.UTF8.GetString(resultArray);

            }
            catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
            return textoEncriptado;
        }


    }
}