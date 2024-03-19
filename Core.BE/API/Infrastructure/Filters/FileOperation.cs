//using Swashbuckle.AspNetCore.Swagger;
//using Swashbuckle.AspNetCore.SwaggerGen;

//namespace Emeint.Core.BE.API.Infrastructure.Filters
//{
//    public class FileOperation : IOperationFilter
//    {
//        public void Apply(Operation operation, OperationFilterContext context)
//        {
//            if (operation.OperationId.ToLower() == "upload_image")
//            {
//                operation.Parameters.Clear();//Clearing parameters
//                operation.Parameters.Add(new NonBodyParameter
//                {
//                    Name = "image",
//                    In = "formData",
//                    Description = "Upload Image",
//                    Required = true,
//                    Type = "file"
//                });
//                operation.Consumes.Add("application/form-data");
//            }
//        }
//    }
//}