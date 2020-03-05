/* Código desenvolvido por: Leandro Amorim Lagoa
 * Linkedin: www.linkedin.com/in/leamorim/
 * E-mail: leamorim@outlook.com
 * */

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

// Classes que nós criaremos
using rsa_application.Configurations;
using rsa_application.Dtos;
using rsa_application.Tools;

using System.Security.Cryptography;
using System.Text;

namespace rsa_application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SegurancaController : ControllerBase
    {
        #region Propriedades
        private RsaAppConfiguration AppConfigurations { get; }
        #endregion

        #region Construtor
        public SegurancaController(RsaAppConfiguration appConfigurations)
        {
            AppConfigurations = appConfigurations;
        }
        #endregion

        #region gerar-chaves
        [HttpGet("gerar-chaves/{keySize?}")]
        public IEnumerable<RsaInfo> Get(int keySize = 2048)
        {
            var listKey = new List<RsaInfo>();
            using (var vRSA = new RSACryptoServiceProvider(keySize))
            {
                // Exporta o array de bytes referente a chave publica somente
                var publicKeyBytes = vRSA.ExportRSAPublicKey();

                // Exporta o array de bytes referente a CHAVE PUBLICA E CHAVE PRIVADA
                var privateKeyBytes = vRSA.ExportRSAPrivateKey();

                // Conversão das chaves geradas para o formato HEXADECIMAL EM TEXTO para ser armazenado
                var publicKeyHexString = CustomConverter.ByteArrayToHex(publicKeyBytes);
                var privateKeyHexString = CustomConverter.ByteArrayToHex(privateKeyBytes);

                // Conversão do mesmo array de bytes para formato de STRING BASE 64 
                var publicKeyBase64String = CustomConverter.ByteArrayToBase64String(publicKeyBytes);
                var privateKeyBase64String = CustomConverter.ByteArrayToBase64String(privateKeyBytes);

                // Adiciono cada uma das formatações a uma lista, mas ambos objetos dizem respeito a mesma chave gerada, 
                // apenas sendo exibida em formatos diferentes
                listKey.Add(new RsaInfo() { FormatName = "Hex", KeySize = keySize, PrivateKey = privateKeyHexString, PublicaKey = publicKeyHexString });
                listKey.Add(new RsaInfo() { FormatName = "Base64", KeySize = keySize, PrivateKey = privateKeyBase64String, PublicaKey = publicKeyBase64String });
            }
            // Retorno o par de chaves geradas e convertida para 2 formatos (HEX e BASE64)
            return listKey;
        }
        #endregion

        #region encrypt
        [HttpPost("encrypt")]
        public string Encrypt([FromBody]string texto)
        {
            // Converto meu texto para array de bytes
            var textBytes = Encoding.UTF8.GetBytes(texto);

            // Busco a chave publica previamente gerada e armazenada no appsettings.json
            var publicKeyBytes = CustomConverter.HexToByteArray(AppConfigurations.PublicaKey);

            // Inicio o provider do .NET CORE que fará o trabalho
            using (var publicRsa = new RSACryptoServiceProvider())
            {
                var bytesRead = 0;
                // Importo a chave publica
                publicRsa.ImportRSAPublicKey(publicKeyBytes, out bytesRead);

                // Executo a criptografia
                var encryptedBytes = publicRsa.Encrypt(textBytes, RSAEncryptionPadding.Pkcs1);

                // Torno meu texto novamente "legível". Aqui optei por uma conversão para HEXADECIMAL, 
                // (pq acho da hora o resultado visual e da uma pressão a mais pra todo mundo que vê rsss)
                // mas você também pode utilizar a string base 64 ou qualquer sistema da sua preferencia.
                // Se usar a string base 64, você deve fazer a da mesma forma no momento de descriptografar
                var encryptedText = CustomConverter.ByteArrayToHex(encryptedBytes);
                return encryptedText;
            }
        }
        #endregion

        #region decrypt
        [HttpPost("decrypt")]
        public string Decrypt([FromBody]string texto)
        {
            // Converto meu texto para array de bytes
            var textBytes = CustomConverter.HexToByteArray(texto);

            // Busco a chave publica previamente gerada e armazenada no appsettings.json
            var privateKeyBytes = CustomConverter.HexToByteArray(AppConfigurations.PrivateKey);

            // Inicio o provider do .NET CORE que fará o trabalho
            using (var privateRsa = new RSACryptoServiceProvider())
            {
                var bytesRead = 0;
                // Importo a chave privada
                privateRsa.ImportRSAPrivateKey(privateKeyBytes, out bytesRead);

                // Executo a descriptografia
                var decryptedBytes = privateRsa.Decrypt(textBytes, RSAEncryptionPadding.Pkcs1);

                // Torno meu texto novamente completamente legível literalmente.
                // mas você também pode utilizar a string base 64 ou qualquer sistema da sua preferencia.
                var plainText = Encoding.UTF8.GetString(decryptedBytes);
                return plainText;
            }
        }
        #endregion
    }
}
