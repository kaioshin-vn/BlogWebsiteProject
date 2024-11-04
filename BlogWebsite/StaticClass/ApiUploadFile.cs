using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;

namespace Client.StaticClass
{
    public static class MinimalApis
    {
        public static RouteGroupBuilder ApiUploadFile(this RouteGroupBuilder router)
        {
            router.MapPost("/uploadImage", async (HttpRequest request, IWebHostEnvironment env) =>
            {
                if (!request.HasFormContentType)
                {
                    return Results.BadRequest("Không phải là định dạng form hợp lệ.");
                }

                var form = await request.ReadFormAsync();
                var files = form.Files; 

                if (files == null || files.Count == 0)
                {
                    return Results.BadRequest("Không có tệp nào được tải lên.");
                }

                var file = files[0];

                // Đường dẫn file UpLoad
                var uploadPath = Path.Combine(env.WebRootPath, "UploadImage");


                var fileName = $"upload-{DateTime.Today.ToString("yyyy-MM-dd")}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                using (var stream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                {
                    // Save the file
                    file.CopyTo(stream);

                    // Return the URL of the file
                    var url = @"UploadImage\" + fileName ;

                    return Results.Ok(new { Url = url });
                }
            });

            router.MapPost("/uploadVideo", async ([FromForm] IFormFile file, IWebHostEnvironment env) =>
            {


                // Đường dẫn file UpLoad
                var uploadPath = Path.Combine(env.WebRootPath, "UploadVideo");


                var fileName = $"upload-{DateTime.Today.ToString("yyyy-MM-dd")}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                using (var stream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                {
                    // Save the file
                    file.CopyTo(stream);

                    // Return the URL of the file
                    var url = @"UploadVideo\" + fileName;

                    return Results.Ok(new { Url = url });
                }
            });


            return router;
        }

        public static RouteGroupBuilder ApiUploadVideo(this RouteGroupBuilder router)
        {
            
            return router;
        }
    }
}
