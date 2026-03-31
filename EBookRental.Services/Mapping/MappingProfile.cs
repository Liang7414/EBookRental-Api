using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EBookRental.Core.Entities;
using EBookRental.Core.DTOs;

namespace EBookRental.Services.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // 1. Book (Entity) -> BookReadDto (DTO)
            CreateMap<Book, BookResponseDTO>()
                // 自定義對應：BookReadDto.CategoryName 來自 Book.Category.Name
                // 如果 Category 是 null（例如忘記 Include），則給予預設值 "無分類"
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "無分類"));

            // 2. IndividualRental (Entity) -> RentalResponseDto (DTO)
            CreateMap<IndividualRental, RentalResponseDto>()
                // 自定義對應：RentalResponseDto.BookTitle 來自 IndividualRental.Book.Title
                // 因為 RentalResponseDto 沒有 BookId，所以我們需要從 IndividualRental 的 Book 導航屬性來取得書名
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book!.Title))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src =>
                    src.ExpiryDate > DateTime.Now ? "使用中" : "已過期"));

            // 3. Subscription (Entity) -> SubscriptionResponseDTO (DTO)
            CreateMap<Subscription, SubscriptionResponseDTO>();
        }
    }
}
