using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace API.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly DataContext _context;

        public VehicleService(DataContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<VehicleDto>> CalculatePremium(VehicleDto vehicleDto) 
        {
            var multipliers = new Multipliers();
            var vehicle = await GetVehicle(vehicleDto.Model);

            if (vehicle == null)
                throw new Exception("Error in Vehicle data");

            multipliers.TypeM = vehicleDto.Type switch
            {
                "Bike" => 1,
                "Cars" => 1.2f,
                _ => throw new Exception("Vehicle Type not specified")
            };
            vehicleDto.Age = vehicleDto.RegYear == DateTime.Now.Year ? 0 : DateTime.Now.Year - vehicleDto.RegYear;
            multipliers.AgeM = vehicleDto.Age switch
            {
                0 or 1 or 2 => 1,
                3 or 4 or 5 => 1.2f,
                _ => 1.5f
            };

            var BikeValue = vehicle.Price * (1 - (multipliers.AgeM - 1));
            float minBikeValue = vehicle.Price * 0.5f;
            vehicleDto.Value = BikeValue;
            vehicleDto.BasePrem = vehicle.Price * 0.1f;

            if (BikeValue == vehicle.Price)
            {
                multipliers.ValueM = 1;
            }
            else if (BikeValue >= minBikeValue)
            {
                multipliers.ValueM = 1.2f;
            }
            else
            {
                multipliers.ValueM = 1.5f;
            }

            string[] nearbyStates = { "AP", "TL", "KL", "KA" };
            string location = vehicleDto.RegNo.ToUpper().Substring(0, 2);

            if (location == "TN")
            {
                multipliers.LocM = 1f;
            }
            else if (nearbyStates.Contains(location))
            {
                multipliers.LocM = 1.1f;
            }
            else
            {
                multipliers.LocM = 1.2f;
            }

            float totalPrem = vehicleDto.BasePrem;
            totalPrem = (float)Math.Round(totalPrem * multipliers.TypeM);
            totalPrem = (float)Math.Round(totalPrem * multipliers.AgeM);
            totalPrem = (float)Math.Round(totalPrem * multipliers.ValueM);
            totalPrem = (float)Math.Round(totalPrem * multipliers.LocM);

            //Get premium for 3 years
            //Third Party
            vehicleDto.TPrem1Year = (totalPrem * 1);
            vehicleDto.TPrem2Year = (totalPrem * 2);
            vehicleDto.TPrem3Year = (totalPrem * 3);

            //Comprehensive
            vehicleDto.CPrem1Year = (float)Math.Round(vehicleDto.TPrem1Year * 1.2f);
            vehicleDto.CPrem2Year = (float)Math.Round(vehicleDto.TPrem2Year * 1.2f);
            vehicleDto.CPrem3Year = (float)Math.Round(vehicleDto.TPrem3Year * 1.2f);

            return vehicleDto;
        }

        public async Task<ActionResult<VehicleDBDto>> Add(VehicleDBDto addVDto)
        {
            if (await VehicleExists(addVDto.Model))
                throw new Exception("Vehicle already exists, Please modify");

            var vehicle = new VehicleData
            {
                Model = addVDto.Model,
                Type = addVDto.Type,
                Price = addVDto.Price,
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return new VehicleDBDto
            {
                Model = addVDto.Model,
                Type = addVDto.Type,
                Price = addVDto.Price,
            };
        }

        private async Task<bool> VehicleExists(string model)
        {
            return await _context.Vehicles.AnyAsync(x => x.Model == model);
        }

        private async Task<VehicleData> GetVehicle(string model)
        {
            return await _context.Vehicles.SingleOrDefaultAsync(x => x.Model == model);
        }

        public byte[] GenerateDoc(VehicleDto vehicleDto)
        {
            string templatePath = "C:\\Users\\ZiPPY\\Desktop\\Comprehensive Plan.docx";
            byte[] templateBytes = File.ReadAllBytes(templatePath);

            using (MemoryStream templateStream = new MemoryStream(templateBytes))
            using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(templateStream, true))
            {
                foreach (var text in wordDocument.MainDocumentPart.Document.Descendants<Text>())
                {
                    if (text.Text.Contains("<<Model>>"))
                        text.Text = text.Text.Replace("<<Model>>", vehicleDto.Model);
                    if (text.Text.Contains("<<RegNo>>"))
                        text.Text = text.Text.Replace("<<RegNo>>", vehicleDto.RegNo);
                    if (text.Text.Contains("<<FullName>>"))
                        text.Text = text.Text.Replace("<<FullName>>", vehicleDto.FullName);
                    if (text.Text.Contains("<<StartDate>>"))
                        text.Text = text.Text.Replace("<<StartDate>>", DateTime.Now.ToString("dd-MM-yyyy"));
                    if (text.Text.Contains("<<EndDate>>"))
                        text.Text = text.Text.Replace("<<EndDate>>", DateTime.Now.ToString("dd-MM-yyyy"));
                }
                MemoryStream generatedDocumentStream = new MemoryStream();
                wordDocument.Clone(generatedDocumentStream);

                return generatedDocumentStream.ToArray();
            }
        }
    }
}
