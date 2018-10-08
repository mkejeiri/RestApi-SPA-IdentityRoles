using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static HttpResponse AddApplicationError(this HttpResponse response, string message)
        {
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            return response;
        }

        public static int CalculateAge(this DateTime dateOfBirth)
        {
            var age = DateTime.Now.Year - dateOfBirth.Year;
            if (dateOfBirth.AddYears(age) > DateTime.Now)
            {
                age--;
            }
            return age;
        }

        public static void AddPagination(this HttpResponse response,
        int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {           
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            
            /*            
            NOTE : JsonSerializerSettings &  CamelCasePropertyNamesContractResolver  
            angular deal with camelcase format otherwise we get in the header:
            Pagination →{"CurrentPage":1,"ItemsPerPage":3,"TotalItems":3,"TotalPages":4}
             and we need : Pagination →{"currentPage":1,"itemsPerPage":3,"totalItems":3,"totalPages":4
            */
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader,camelCaseFormatter));
            //we need also expose the Pagination header (like the errors in AddApplicationError)
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}