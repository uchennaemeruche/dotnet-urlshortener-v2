using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UrlShortener.Entities
{
	public class UrlCode
	{
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public bool? IsUsed { get; set; } = false;

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

    }
}

