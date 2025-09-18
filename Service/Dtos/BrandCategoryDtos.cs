namespace ApplicationTest.Dtos;

public record BrandCreateDto(string Name);
public record BrandUpdateDto(Guid BrandId, string Name);
public record BrandView(Guid BrandId, string Name);

public record CategoryCreateDto(string Name);
public record CategoryUpdateDto(Guid CategoryId, string Name);
public record CategoryView(Guid CategoryId, string Name);
