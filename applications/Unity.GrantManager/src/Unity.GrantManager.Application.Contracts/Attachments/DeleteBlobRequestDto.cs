﻿using System.ComponentModel.DataAnnotations;

namespace Unity.GrantManager.Attachments
{
    public class DeleteBlobRequestDto
    {
        [Required]
        public string S3ObjectKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
