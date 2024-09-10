using Cryptography.Web.Services;
using Crytography.Models;
using Crytography.Services;
using Crytography.Web.Models;
using Crytography.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
                } else if (action == "decrypt" && encryptedText != null)
                {
                    ViewBag.DecryptedText = LabTwoService.Decrypt(encryptedText, model.D, model.N);
                }
            }

            TempData["EncryptedText"] = encryptedText;

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
                    ViewBag.EncryptedText = LabThreeService.Encrypt(inputText, model.Kx, model.N);
                }
                else if (action == "decrypt")
                {
                    if (!string.IsNullOrEmpty(encryptedText) && encryptedText != null)
                    {
                        ViewBag.DecryptedText = LabThreeService.Decrypt(encryptedText, model.Ky, model.N);
                    }
                }
            }

            TempData["EncryptedText"] = encryptedText;
            return View(model);
        }

    }
}
