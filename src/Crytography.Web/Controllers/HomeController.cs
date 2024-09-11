using Cryptography.Web.Services;
using Crytography.Models;
using Crytography.Services;
using Crytography.Web.Models;
using Crytography.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace Crytography.Controllers
{
    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Lab1()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Lab1(string key, string inputText)
        {
            if (!string.IsNullOrEmpty(inputText))
            {
                string encryptedText = await LabOneService.EncryptAsync(inputText, key);
                ViewBag.Key = key;
                ViewBag.InputText = inputText;
                ViewBag.EncryptedText = encryptedText;
            }
            return View();
        }

        public IActionResult Lab2()
        {
            var model = new Lab2Model();

            return View(model);
        }

        [HttpPost]
        public IActionResult Lab2(Lab2Model model, string action, string inputText)
        {
            var encryptedText = TempData["EncryptedText"] as string;

            if (action == "generateKeys")
            {
                var keysModel = LabTwoService.GenerateKeys();
                model = new Lab2Model(keysModel.P, keysModel.Q, keysModel.N, keysModel.E, keysModel.D, keysModel.Y);
            }

            if (!string.IsNullOrEmpty(inputText))
            {
                ViewBag.InputText = inputText;

                if (action == "encrypt")
                {
                    encryptedText = LabTwoService.Encrypt(inputText, model.E, model.N);
                    ViewBag.EncryptedText = encryptedText;
                    TempData["EncryptedText"] = encryptedText;
                }
                else if (action == "decrypt" && encryptedText != null)
                {
                    ViewBag.DecryptedText = LabTwoService.Decrypt(encryptedText, model.D, model.N);
                }
            }



            return View(model);
        }

        public IActionResult Lab3()
        {

            var model = new Lab3Model();

            return View(model);
        }

        [HttpPost]
        public IActionResult Lab3(Lab3Model model, string action, string inputText)
        {
            var encryptedText = TempData["EncryptedText"] as string;

            if (action == "generateKeys")
            {
                var keysModel = LabThreeService.GenerateKeys();
                model = new Lab3Model(keysModel.N, keysModel.Q, keysModel.X, keysModel.Y, keysModel.Kx, keysModel.Ky, keysModel.A, keysModel.B);
            }

            if (!string.IsNullOrEmpty(inputText))
            {
                ViewBag.InputText = inputText;

                if (action == "encrypt")
                {
                    // Encrypt the input text and store it in TempData for future decryption
                    encryptedText = LabThreeService.Encrypt(inputText, model.Kx, model.N);
                    TempData["EncryptedText"] = encryptedText;
                    ViewBag.EncryptedText = encryptedText;
                }
                else if (action == "decrypt" && encryptedText != null)
                {
                    // Decrypt the text stored in TempData
                    ViewBag.DecryptedText = LabThreeService.Decrypt(encryptedText, model.Ky, model.N);
                }
            }

            return View(model);
        }

        // Метод для отображения страницы с формой
        [HttpGet]
        public IActionResult Lab4()
        {
            var model = new Lab4Model();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EncryptFile(Lab4Model model)
        {
            // Validate file inputs
            if (model.EncryptedFilePath == null || string.IsNullOrWhiteSpace(model.EncryptedFilePathSave) || string.IsNullOrEmpty(model.Key))
            {
                ModelState.AddModelError(string.Empty, "Пожалуйста, выберите файл, введите имя для сохранения и ключ.");
                Console.WriteLine("Key: " + model.Key);
                return View("Lab4", model);
            }

            Console.WriteLine("Key2: " + model.Key);

            // Read the uploaded file into a byte array
            byte[] fileBytes;
            using (var inputStream = model.EncryptedFilePath.OpenReadStream())
            {
                fileBytes = await ReadFullyAsync(inputStream);
            }

            // Convert the file bytes to a string for encryption
            var plaintext = System.Text.Encoding.UTF8.GetString(fileBytes);

            // Encrypt the file content
            var encryptedText = Lab4Service.Encrypt(plaintext, model.Key);

            // Convert the encrypted text back to bytes
            var encryptedBytes = System.Text.Encoding.UTF8.GetBytes(encryptedText);

            // Return the encrypted file directly to the user for download
            return File(encryptedBytes, "application/octet-stream", model.EncryptedFilePathSave);
        }

        [HttpPost]
        public async Task<IActionResult> DecryptFile(Lab4Model model)
        {
            if (model.DecryptedFilePath == null || string.IsNullOrWhiteSpace(model.DecryptedFileSavePath) || string.IsNullOrEmpty(model.Key))
            {
                Console.WriteLine($"{model.DecryptedFilePath} : {model.DecryptedFileSavePath} : {model.Key}");
                ModelState.AddModelError(string.Empty, "Пожалуйста, выберите файл, введите имя для сохранения и ключ.");
                return View("Lab4", model);
            }

            Console.WriteLine("Key2: " + model.Key);

            // Read the uploaded file into a byte array
            byte[] fileBytes;
            using (var inputStream = model.DecryptedFilePath.OpenReadStream())
            {
                fileBytes = await ReadFullyAsync(inputStream);
            }

            // Convert the file bytes to a string for decryption
            var encryptedText = System.Text.Encoding.UTF8.GetString(fileBytes);

            // Decrypt the file content
            var decryptedText = Lab4Service.Decrypt(encryptedText, model.Key);

            // Convert the decrypted text back to bytes
            var decryptedBytes = System.Text.Encoding.UTF8.GetBytes(decryptedText);

            // Return the decrypted file directly to the user for download
            return File(decryptedBytes, "application/octet-stream", model.DecryptedFileSavePath);
        }

        // Helper method to read a stream into a byte array
        private static async Task<byte[]> ReadFullyAsync(Stream input)
        {
            using (var memoryStream = new MemoryStream())
            {
                await input.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }



    }
}
