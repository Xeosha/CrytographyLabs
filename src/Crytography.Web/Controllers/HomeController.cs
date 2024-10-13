using Cryptography.Web.Services;
using Crytography.Services;
using Crytography.Web.Models;
using Crytography.Web.Services;
using Crytography.Web.Services.Lab6Services;
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
        public IActionResult Lab1(string key, string inputText)
        {
            if (!string.IsNullOrEmpty(inputText))
            {
                string encryptedText = LabOneService.Encrypt(inputText, key);
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
                encryptedText = null;
            }

            if (!string.IsNullOrEmpty(inputText))
            {
                ViewBag.InputText = inputText;

                if (action == "encrypt")
                {
                    encryptedText = LabTwoService.Encrypt(inputText, model.E, model.N);
                }
                else if (action == "decrypt" && !string.IsNullOrEmpty(encryptedText))
                {
                    ViewBag.DecryptedText = LabTwoService.Decrypt(encryptedText, model.D, model.N);
                }
            }

            TempData["EncryptedText"] = encryptedText;
            ViewBag.EncryptedText = encryptedText;



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
                encryptedText = null;
            }

            if (!string.IsNullOrEmpty(inputText))
            {
                ViewBag.InputText = inputText;

                if (action == "encrypt")
                {
                    encryptedText = LabThreeService.Encrypt(inputText, model.Kx, model.N);
                }
                else if (action == "decrypt" && !string.IsNullOrEmpty(encryptedText))
                {
                    ViewBag.DecryptedText = LabThreeService.Decrypt(encryptedText, model.Ky, model.N);
                }
            }

            TempData["EncryptedText"] = encryptedText;
            ViewBag.EncryptedText = encryptedText;

            return View(model);
        }

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
                ModelState.AddModelError(string.Empty, "����������, �������� ����, ������� ��� ��� ���������� � ����.");
                return View("Lab4", model);
            }

            byte[] fileBytes;
            using (var inputStream = model.EncryptedFilePath.OpenReadStream())
            {
                fileBytes = await ReadFullyAsync(inputStream);
            }

            var plaintext = System.Text.Encoding.UTF8.GetString(fileBytes);

            var encryptedText = Lab4Service.Encrypt(plaintext, Encoding.UTF8.GetBytes(model.Key));

            var encryptedBytes = System.Text.Encoding.UTF8.GetBytes(encryptedText);

            return File(encryptedBytes, "application/octet-stream", model.EncryptedFilePathSave);
        }

        [HttpPost]
        public async Task<IActionResult> DecryptFile(Lab4Model model)
        {
            if (model.DecryptedFilePath == null || string.IsNullOrWhiteSpace(model.DecryptedFileSavePath) || string.IsNullOrEmpty(model.Key))
            {
                ModelState.AddModelError(string.Empty, "����������, �������� ����, ������� ��� ��� ���������� � ����.");
                return View("Lab4", model);
            }


            byte[] fileBytes;
            using (var inputStream = model.DecryptedFilePath.OpenReadStream())
            {
                fileBytes = await ReadFullyAsync(inputStream);
            }

            var encryptedText = System.Text.Encoding.UTF8.GetString(fileBytes);

            var decryptedText = Lab4Service.Decrypt(encryptedText, Encoding.UTF8.GetBytes(model.Key));

            var decryptedBytes = System.Text.Encoding.UTF8.GetBytes(decryptedText);

            return File(decryptedBytes, "application/octet-stream", model.DecryptedFileSavePath);
        }

        private static async Task<byte[]> ReadFullyAsync(Stream input)
        {
            using (var memoryStream = new MemoryStream())
            {
                await input.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }


        [HttpGet]
        public IActionResult Lab5()
        {
            var model = new Lab5Model();
            return View(model);
        }

        [HttpPost]
        public IActionResult Lab5(Lab5Model model)
        {
        // ���������, ��� �������� ������������������ ������� �� 8 ���
            if (model.InputCode.Length != 8 || !Lab5Service.IsBinaryString(model.InputCode))
            {
                ModelState.AddModelError("", "������� ���������� 8-������ ������������������ (������ 0 � 1).");
                return View("Lab5", model);
            }

            // ������������ ��� ��������
            int parityBit = Lab5Service.CalculateParityBit(model.InputCode);

            // ������������� ���������� ������������������ � ��� ��������
            model.EditableCode = model.InputCode;
            model.OldParityBit = model.ParityBit = parityBit;

            // ������� ��������� ��������
            model.CheckPerformed = false;
            model.HasError = false;

            return View("Lab5", model);
        }

        [HttpPost]
        public IActionResult CheckCode(Lab5Model model)
        {
            // ���������, ��� ������������� ����� ������� �� 8 ���
            if (model.EditableCode.Length != 8 || !Lab5Service.IsBinaryString(model.EditableCode))
            {
                ModelState.AddModelError("", "���������� ����� ������ ��������� 8 ��� (������ 0 � 1).");
                return View("Lab5", model);
            }

            // ������������ ��������� ��� ��������
            model.ParityBit = Lab5Service.CalculateParityBit(model.EditableCode);

            // ��������� ������� ������
            model.HasError = model.OldParityBit != model.ParityBit;
            model.CheckPerformed = true;    

            return View("Lab5", model);
        }

        [HttpGet]
        public IActionResult Lab6()
        {
            var model = new Lab4Model();
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> CompressFile(string CompressionMethod, IFormFile FileToCompress, string compressedFilePathSave)
        {
            if (FileToCompress == null || FileToCompress.Length == 0)
            {
                return BadRequest("���� ��� ������ �� ������.");
            }


            byte[] fileBytes;
            using (var inputStream = FileToCompress.OpenReadStream())
            {
                fileBytes = await ReadFullyAsync(inputStream);
            }

            ICoder coder;

            if (CompressionMethod == "Arithmetic")
            {
                coder = new ArithmeticCoder();
            }
            else if (CompressionMethod == "LZ77")
            {
                coder = new Lz77Coder();
            }
            else
            {
                coder = new DoubleCoder(new ArithmeticCoder(), new Lz77Coder());
            }

            var compress = coder.Encode(fileBytes);

            return File(compress, "application/octet-stream", compressedFilePathSave + ".dat");
        }


        [HttpPost]
        public async Task<IActionResult> DecompressFile(string DeCompressionMethod, IFormFile FileToDeCompress, string decompressedFilePathSave)
        {
            if (DeCompressionMethod == null || FileToDeCompress.Length == 0)
            {
                return BadRequest("���� ��� ���������� �� ������.");
            }

            if (string.IsNullOrWhiteSpace(decompressedFilePathSave))
            {
                return BadRequest("������� ��� ��� ���������� �������������� �����.");
            }


            byte[] fileBytes;
            using (var inputStream = FileToDeCompress.OpenReadStream())
            {
                fileBytes = await ReadFullyAsync(inputStream);
            }

            ICoder coder;

            if (DeCompressionMethod == "Arithmetic")
            {
                coder = new ArithmeticCoder();
            }
            else if (DeCompressionMethod == "LZ77")
            {
                coder = new Lz77Coder();
            }
            else
            {
                coder = new DoubleCoder(new ArithmeticCoder(), new Lz77Coder());
            }

            var decompress = coder.Decode(fileBytes);

            return File(decompress, "application/octet-stream", decompressedFilePathSave + ".txt");
        }

        [HttpGet]
        public IActionResult Lab7()
        {
            return View(new Lab7Model());
        }

        [HttpPost]
        public IActionResult HashPassword(Lab7Model model)
        {
            try
            {
                var hashedPassword = Lab7Service.HashPassword(model.Password);
                model.HashedPassword = hashedPassword;
                model.ErrorMessage = null; // ���������� ��������� �� ������
            }
            catch (ArgumentException ex)
            {
                model.HashedPassword = null; // ���������� ���
                model.ErrorMessage = ex.Message;
            }

            return View("Lab7", model);
        }

        [HttpPost]
        public IActionResult VerifyPassword(Lab7Model model)
        {
            var result = Lab7Service.VerifyPassword(model.CheckPassword, model.HashedPasswordInput);
            model.VerificationResult = result ? "������ ���������." : "������ �����������.";

            return View("Lab7", model);
        }

        [HttpGet]
        public IActionResult Lab8()
        {
            return View(new Lab8Model());
        }

        [HttpPost]
        public IActionResult Encrypt(Lab8Model model)
        {
            try
            {
                // ��������� �����
                int blockCount = (Encoding.UTF8.GetByteCount(model.Plaintext) + 3) / 4; // ���������� ������ �� 4 �����
                var gammaList = Lab8Service.GenerateListGamma(blockCount); // ��������� �����

                // ���������� ������
                model.Ciphertext = Lab8Service.Encrypt(model.Plaintext, gammaList);

                Console.WriteLine(model.Ciphertext);    

                model.GammaList = string.Join(", ", gammaList); // �������������� ����� � ������
                model.ErrorMessage = null; // ���������� ��������� �� ������
            }
            catch (Exception ex)
            {
                model.Ciphertext = null; // ���������� ����������
                model.ErrorMessage = ex.Message;
            }

            return View("Lab8", model);
        }

        [HttpPost]
        public IActionResult Decrypt(Lab8Model model)
        {
            try
            {
                // �������������� ����� ������� � ������ uint
                var gammaList = Array.ConvertAll(model.GammaList.Split(", "), uint.Parse);

                // ������������� ������
                model.DecryptedText = Lab8Service.Decrypt(model.Ciphertext, gammaList);

                Console.WriteLine(model.DecryptedText);

                model.ErrorMessage = null; // ���������� ��������� �� ������
            }
            catch (Exception ex)
            {
                model.DecryptedText = null; // ���������� �������������� �����
                model.ErrorMessage = ex.Message;
            }

            return View("Lab8", model);
        }
    }

}
