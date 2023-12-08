using System.ComponentModel.DataAnnotations;

namespace EstanteManiaWebAssembly.Models.DTO_s
{
    public class AuthorDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The author name is required.")]
        [MinLength(3)]
        [MaxLength(80)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "The author description is required.")]
        [MinLength(5)]
        [MaxLength(250)]
        public string? Description { get; set; }
    }
}