using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InternalIssues.Extensions
{
    public class AllowedExtensions : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensions(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validatoinContext)
        {
            var file = value as IFormFile;
            if(file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower(extension)))
                {
                    return new ValidationResult(GetErrorMessage(extension));
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage(string ext)
        {
            return $"the File extension {ext} is not allowed!";
        }
    }
}
