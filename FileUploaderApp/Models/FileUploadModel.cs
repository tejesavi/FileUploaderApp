using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FileUploaderApp.Models
{
    public class FileUploadModel
    {
        [Required(ErrorMessage = "Please select a file.")]
        public IFormFile File { get; set; }
    }
}
