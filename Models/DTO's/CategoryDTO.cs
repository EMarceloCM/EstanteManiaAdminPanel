using System.ComponentModel.DataAnnotations;

namespace EstanteManiaWebAssembly.Models.DTO_s
{
    public class CategoryDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The name is required.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "The description is required.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "The icon name is required")]
        public string? IconCSS { get; set; }
    }
}