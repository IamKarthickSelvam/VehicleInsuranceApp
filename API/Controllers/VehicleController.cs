using API.DTOs;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using API.Interfaces;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace API.Controllers
{
    public class VehicleController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly IVehicleService _vehicleService;

        public VehicleController(DataContext context, IVehicleService vehicleService)
        {
            _context = context;
            _vehicleService = vehicleService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetBasicDetails()
        {
            try
            {
                var Vehicles = _context.Vehicles.Select(x => x.Model).ToList();
                return Vehicles;
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("calculate")]
        public async Task<ActionResult<VehicleDto>> CalculatePremium([FromBody]VehicleDto vehicleDto)
        {
            try
            {
                var response = await _vehicleService.CalculatePremium(vehicleDto);
                return response;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("generate")]
        public ActionResult Generate([FromBody] VehicleDto vehicleDto)
        {
            try
            {
                byte[] invoiceBytes = _vehicleService.GenerateDoc(vehicleDto);
                return File(invoiceBytes, "application/blob", "Invoice.docx");
            }
            catch (Exception ex)
            {
                return BadRequest("Error in generating file: " + ex.Message);
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult<VehicleDBDto>> Add(VehicleDBDto addVDto)
        {
            try
            {
                var response = await _vehicleService.Add(addVDto);
                return response;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("pdf")]
        public async Task<ActionResult<VehicleDto>> GeneratePDF([FromBody] VehicleDto vehicleDto)
        {
            var document = new PdfDocument();
            string htmlTemplatePath = "C:\\Users\\ZiPPY\\Desktop\\CP.html";
            string HtmlContent = System.IO.File.ReadAllText(htmlTemplatePath);

            if (vehicleDto.SelectedPlan.Substring(vehicleDto.SelectedPlan.IndexOf('-') + 1, 2) == "CP")
            {
                HtmlContent = HtmlContent.Replace("{{InsType}}", "Comprehensive");
            }
            else
            {
                HtmlContent = HtmlContent.Replace("{{InsType}}", "Third Party");
            }
            HtmlContent = HtmlContent.Replace("{{Type}}", vehicleDto.Type);
            HtmlContent = HtmlContent.Replace("{{Model}}", vehicleDto.Model);
            HtmlContent = HtmlContent.Replace("{{RegNo}}", vehicleDto.RegNo);
            HtmlContent = HtmlContent.Replace("{{fullName}}", vehicleDto.FullName);
            HtmlContent = HtmlContent.Replace("{{StartDate}}", DateTime.Now.ToString("dd-MM-yyyy"));                         
            HtmlContent = HtmlContent.Replace("{{EndDate}}", DateTime.Now.ToString("dd-MM-yyyy"));

            PdfGenerator.AddPdfPages(document, HtmlContent, PdfSharpCore.PageSize.A4);
            byte[] response = null;
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms);
                response = ms.ToArray();
            }
            string Filename = "Test.pdf";
            return File(response, "application/pdf", Filename);
        }

        //FIGURE OUT DELETE ASAP
        //[HttpDelete]
        //public async Task<ActionResult> Delete(VehicleDBDto vehicleDBDto)
        //{
        //    if (!await VehicleExists(vehicleDBDto.Model))
        //        return BadRequest("Vehicle does not exist");

        //    var vehicle = new VehicleData
        //    {
        //        Model = vehicleDBDto.Model,
        //        Type = vehicleDBDto.Type,
        //        Price = vehicleDBDto.Price,
        //    };

        //    _context.Vehicles.Remove(vehicle);
        //    await _context.SaveChangesAsync();

        //    return StatusCode(200);
        //}
    }
}
