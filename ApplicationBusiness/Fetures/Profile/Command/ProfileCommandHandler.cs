using Application.Abstraction.message;
using Application.Fetures.Authentication.Command.Models;
using ApplicationBusiness.Fetures.Profile.Command.Models;
using Domain.Abstraction;
using Domain.BaseResponce;
using Domain.Entity.Identity;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.Profile.Command
{
    internal class ProfileTravelCompanyCommandHandler : ICommandHandler<CreateTravelerCompanyProfileCommand, ApiResponse>,
        ICommandHandler<UpdateTravelerCompanyProfileCommand, ApiResponse>
    {
        private IWriteUnitOfWork _writeUnitOfWork;
        private IWriteGenericRepo<TravelCompany> _WTR;
        private IReadGenericRepo<TravelCompany> _RTR;


        public ISender Sender { get; set; }



        public ProfileTravelCompanyCommandHandler(IWriteUnitOfWork writeUnitOfWork, IWriteGenericRepo<TravelCompany> wTR, IReadGenericRepo<TravelCompany> rTR, ISender sender)
        {
            _writeUnitOfWork = writeUnitOfWork;
            _WTR = wTR;
            _RTR = rTR;
            Sender = sender;
        }

        public async Task<ApiResponse> Handle(CreateTravelerCompanyProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _writeUnitOfWork.BeginTransactionAsync();
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploadsVerification");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);
                var baseUrl = $"http://rahalbk.runasp.net";

                string SaveFile(IFormFile file, string prefix)
                {
                    var extension = Path.GetExtension(file.FileName); // keep original extension
                    var fileName = $"{prefix}{extension}";
                    var fullPath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    // Return public URL
                    return $"{baseUrl}/profile/{fileName}";
                }

                var entity = new TravelCompany
                {
                    PhotoUrl = request.dto.Photo,

                    Id = request.Id,
                    Ssn = request.dto.Ssn,
                    UserId = request.Id,
                    Bio = request.dto.Bio,
                    //travelCompanyBusinessGalaries = request.dto.BusinessGalaries.Select(s => new TravelCompanyBusinessGalary
                    //{
                    //    PhotoUrl = s.PhotoUrl,
                    //    Date = s.Date,
                    //    Description = s.Description,
                    //    Location = s.Location,
                    //}).ToList(),
                    travelCompanyBusinessGalaries = new List<TravelCompanyBusinessGalary>{
                        new TravelCompanyBusinessGalary
                        {
                        PhotoUrl = SaveFile(request.dto.BusinessGalaries.Photo,$"{request.dto.Ssn.TakeLast(4)}-{DateTime.UtcNow:yyyyMMddHHmmss}"),
                        Date = request.dto.BusinessGalaries.Date,
                        Description = request.dto.BusinessGalaries.Description,
                        Location = request.dto.BusinessGalaries.Location,
                        }
                    },

                    traveleCompanyAddresses = request.dto.Adresses.Select(s => new TravelerCompanyAddress
                    {
                        BuildingNumber = s.BuildingNumber,
                        City = s.City,
                        Street = s.Street,
                        Country = s.Country,
                    }).ToList()

                };
                await _WTR.AddAsync(entity);

                await Sender.Send(new VerifiedUser(request.Id));


                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();
                var temp = new TemplateTravelComapny
                {
                    PhotoUrl = entity.PhotoUrl,
                    Id = entity.Id,
                    Ssn = entity.Ssn,
                    Bio = entity.Bio,
                    BusinessGalaries = entity.travelCompanyBusinessGalaries.Select(s => new Dtos.Profile.BusinessGalaryDto
                    {
                        PhotoUrl = s.PhotoUrl,
                        Date = s.Date,
                        Description = s.Description,
                        Location = s.Location,
                    }).ToList(),
                    Adresses = entity.traveleCompanyAddresses.Select(s => new Dtos.Profile.Adress
                    {
                        BuildingNumber = s.BuildingNumber,
                        City = s.City,
                        Street = s.Street,
                        Country = s.Country,
                    }).ToList()
                };



                return new ApiResultResponse<TemplateTravelComapny>((int)HttpStatusCode.Created, temp, "Profile created successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        public async Task<ApiResponse> Handle(UpdateTravelerCompanyProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _writeUnitOfWork.BeginTransactionAsync();
                var tComp = await _RTR.GetAll().Include(x=>x.travelCompanyBusinessGalaries).Include(x=>x.traveleCompanyAddresses).FirstOrDefaultAsync(x=>x.Id== request.Id);
                if (tComp != null)
                    return new ApiResponse(404, "There's no profile to User");
                tComp.Ssn = request.dto.Ssn;
                tComp.Bio = request.dto.Bio;


                //tComp.travelCompanyBusinessGalaries = new List<TravelCompanyBusinessGalary>{
                //        new TravelCompanyBusinessGalary
                //    {
                //        Date = request.dto.BusinessGalaries.Date,
                //        Description = request.dto.BusinessGalaries.Description,
                //        Location = request.dto.BusinessGalaries.Location,
                //        }
                //    };
                tComp.traveleCompanyAddresses = request.dto.Adresses.Select(s => new TravelerCompanyAddress
                {
                    BuildingNumber = s.BuildingNumber,
                    City = s.City,
                    Street = s.Street,
                    Country = s.Country,
                }).ToList();
                await _WTR.UpdateAsync(tComp, request.Id);

                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();
                var temp = new TemplateTravelComapny
                {
                    Id = tComp.Id,
                    Ssn = tComp.Ssn,
                    Bio = tComp.Bio,
                    BusinessGalaries = tComp.travelCompanyBusinessGalaries.Select(s => new Dtos.Profile.BusinessGalaryDto
                    {
                        PhotoUrl = s.PhotoUrl,
                        Date = s.Date,
                        Description = s.Description,
                        Location = s.Location,
                    }).ToList(),
                    Adresses = tComp.traveleCompanyAddresses.Select(s => new Dtos.Profile.Adress
                    {
                        BuildingNumber = s.BuildingNumber,
                        City = s.City,
                        Street = s.Street,
                        Country = s.Country,
                    }).ToList()
                };
                return new ApiResultResponse<TemplateTravelComapny>((int)HttpStatusCode.OK, temp, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
    internal class ProfileTourGiudeCommandHandler : ICommandHandler<CreateTourGuideProfileCommand, ApiResponse>,
        ICommandHandler<UpdateTourGuideProfileCommand, ApiResponse>
    {
        IWriteUnitOfWork _writeUnitOfWork;
        IWriteGenericRepo<TourGuide> _WTR;
        IReadGenericRepo<TourGuide> _RTR;
        public ISender Sender { get; set; }


        public ProfileTourGiudeCommandHandler(IWriteUnitOfWork writeUnitOfWork, IWriteGenericRepo<TourGuide> wTR, IReadGenericRepo<TourGuide> rTR, ISender sender)
        {
            _writeUnitOfWork = writeUnitOfWork;
            _WTR = wTR;
            _RTR = rTR;
            Sender = sender;
        }

        public async Task<ApiResponse> Handle(UpdateTourGuideProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _writeUnitOfWork.BeginTransactionAsync();
                var tComp = await _RTR.GetByIdAsync(request.Id);
                tComp.SalaryPerDay = request.dto.SalaryPerDay;
                tComp.Ssn = request.dto.Ssn;
                tComp.Bio = request.dto.Bio;
                tComp.tourGuidAddresses = request.dto.Adresses.Select(s => new TourGuideAddress
                {
                    BuildingNumber = s.BuildingNumber,
                    City = s.City,
                    Street = s.Street,
                    Country = s.Country,
                }).ToList();
                await _WTR.UpdateAsync(tComp, request.Id);

                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();
                var temp = new TemplateTourGuide
                {
                    Id = tComp.Id,
                    SalaryPerDay = tComp.SalaryPerDay,
                    Ssn = tComp.Ssn,
                    Bio = tComp.Bio,
                    BusinessGalaries = tComp.tourGuidBusinessGalaries.Select(s => new Dtos.Profile.BusinessGalaryDto
                    {
                        PhotoUrl = s.PhotoUrl,
                        Date = s.Date,
                        Description = s.Description,
                        Location = s.Location,
                    }).ToList(),
                    Adresses = tComp.tourGuidAddresses.Select(s => new Dtos.Profile.Adress
                    {
                        BuildingNumber = s.BuildingNumber,
                        City = s.City,
                        Street = s.Street,
                        Country = s.Country,
                    }).ToList()
                };
                return new ApiResultResponse<TemplateTourGuide>((int)HttpStatusCode.OK, temp, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(CreateTourGuideProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _writeUnitOfWork.BeginTransactionAsync();

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploadsVerification");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);
                var baseUrl = $"http://rahalbk.runasp.net";

                string SaveFile(IFormFile file, string prefix)
                {
                    var extension = Path.GetExtension(file.FileName); // keep original extension
                    var fileName = $"{prefix}{extension}";
                    var fullPath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    // Return public URL
                    return $"{baseUrl}/profile/{fileName}";
                }

                var entity = new TourGuide
                {

                    PhotoUrl = request.dto.Photo,
                    SalaryPerDay = request.dto.SalaryPerDay,
                    Id = request.Id,
                    Ssn = request.dto.Ssn,
                    UserId = request.Id,
                    Bio = request.dto.Bio,
                    tourGuidBusinessGalaries = new List<TourGuideBusinessGalary>{
                        new TourGuideBusinessGalary
                    {
                        PhotoUrl = SaveFile(request.dto.BusinessGalaries.Photo,$"{request.dto.Ssn.TakeLast(4)}-{DateTime.UtcNow:yyyyMMddHHmmss}"),
                        Date = request.dto.BusinessGalaries.Date,
                        Description = request.dto.BusinessGalaries.Description,
                        Location = request.dto.BusinessGalaries.Location,
                        }
                    },
                    tourGuidAddresses = request.dto.Adresses.Select(s => new TourGuideAddress
                    {
                        BuildingNumber = s.BuildingNumber,
                        City = s.City,
                        Street = s.Street,
                        Country = s.Country,
                    }).ToList()
                };
                await _WTR.AddAsync(entity);
                await _writeUnitOfWork.SaveChangesAsync();
                await Sender.Send(new VerifiedUser(request.Id));

                await _writeUnitOfWork.CommitAsync();
                var temp = new TemplateTourGuide
                {
                 PhotoUrl=entity.PhotoUrl,   
                    Id = entity.Id,
                    SalaryPerDay = entity.SalaryPerDay,
                    Ssn = entity.Ssn,
                    Bio = entity.Bio,
                    BusinessGalaries = entity.tourGuidBusinessGalaries.Select(s => new Dtos.Profile.BusinessGalaryDto
                    {
                        PhotoUrl = s.PhotoUrl,
                        Date = s.Date,
                        Description = s.Description,
                        Location = s.Location,
                    }).ToList(),
                    Adresses = entity.tourGuidAddresses.Select(s => new Dtos.Profile.Adress
                    {
                        BuildingNumber = s.BuildingNumber,
                        City = s.City,
                        Street = s.Street,
                        Country = s.Country,
                    }).ToList()
                };
                return new ApiResultResponse<TemplateTourGuide>((int)HttpStatusCode.Created, temp, "Profile created successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
    internal class ProfileTravelerCommandHandler : ICommandHandler<CreateTravelerProfileCommand, ApiResponse>,
        ICommandHandler<UpdateTravelerProfileCommand, ApiResponse>
    {
        IWriteUnitOfWork _writeUnitOfWork;
        IWriteGenericRepo<Traveler> _WTR;
        IReadGenericRepo<Traveler> _RTR;
        public ISender Sender { get; set; }


        public ProfileTravelerCommandHandler(IWriteGenericRepo<Traveler> wTR, IWriteUnitOfWork writeUnitOfWork, IReadGenericRepo<Traveler> rTR, ISender sender)
        {
            _WTR = wTR;
            _writeUnitOfWork = writeUnitOfWork;
            _RTR = rTR;
            Sender = sender;
        }

        public async Task<ApiResponse> Handle(UpdateTravelerProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _writeUnitOfWork.BeginTransactionAsync();
                var tComp = await _RTR.GetByIdAsync(request.Id);
                tComp.Ssn = request.dto.Ssn;
                tComp.Bio = request.dto.Bio;

                tComp.trvelerAddresses = request.dto.Adresses.Select(s => new TrvelerAddress
                {
                    BuildingNumber = s.BuildingNumber,
                    City = s.City,
                    Street = s.Street,
                    Country = s.Country,
                }).ToList();
                await _WTR.UpdateAsync(tComp, request.Id);
                await _writeUnitOfWork.SaveChangesAsync();
                await _writeUnitOfWork.CommitAsync();
                var temp = new TemplateTraveler
                {
                    Id = tComp.Id,
                    Ssn = tComp.Ssn,
                    Bio = tComp.Bio,
                    Adresses = tComp.trvelerAddresses.Select(s => new Dtos.Profile.Adress
                    {
                        BuildingNumber = s.BuildingNumber,
                        City = s.City,
                        Street = s.Street,
                        Country = s.Country,
                    }).ToList()
                };
                return new ApiResultResponse<TemplateTraveler>((int)HttpStatusCode.OK, temp, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<ApiResponse> Handle(CreateTravelerProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _writeUnitOfWork.BeginTransactionAsync();
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploadsVerification");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);
                var baseUrl = $"http://rahalbk.runasp.net";
                string SaveFile(IFormFile file, string prefix)
                {
                    var extension = Path.GetExtension(file.FileName); // keep original extension
                    var fileName = $"{prefix}{extension}";
                    var fullPath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    // Return public URL
                    return $"{baseUrl}/profile/{fileName}";
                }
                var entity = new Traveler
                {
                    PhotoUrl = request.dto.Photo,
                    Id = request.Id,
                    Ssn = request.dto.Ssn,
                    UserId = request.Id,
                    Bio = request.dto.Bio,
                    trvelerAddresses = request.dto.Adresses.Select(s => new TrvelerAddress
                    {
                        BuildingNumber = s.BuildingNumber,
                        City = s.City,
                        Street = s.Street,
                        Country = s.Country,
                    }).ToList()

                };
                await _WTR.AddAsync(entity);
                var newtoken = await Sender.Send(new VerifiedUser(request.Id));
                await _writeUnitOfWork.SaveChangesAsync();

                await _writeUnitOfWork.CommitAsync();
                var temp = new TemplateTraveler
                {
                    PhotoUrl = entity.PhotoUrl,
                    Id = entity.Id,
                    Ssn = entity.Ssn,
                    Bio = entity.Bio,
                    Adresses = entity.trvelerAddresses.Select(s => new Dtos.Profile.Adress
                    {
                        BuildingNumber = s.BuildingNumber,
                        City = s.City,
                        Street = s.Street,
                        Country = s.Country,
                    }).ToList()
                };
                return new ApiResultResponse<TemplateTraveler>((int)HttpStatusCode.Created, temp, "Profile created successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
