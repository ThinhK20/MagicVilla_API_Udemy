﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Web.Models.Dto
{
	public class VillaNumberUpdateDTO
	{
		[Required]
		public int VillaNo { get; set; }
		[Required]
		public int VillaID { get; set; }

		[DisplayName("Special Details")]
		public string SpecialDetails { get; set; }
	}
}
