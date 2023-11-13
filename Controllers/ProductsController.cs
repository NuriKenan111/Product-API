using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductApi.DTO;
using ProductApi.Models;

namespace ProductApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductsContext _context;

    public ProductsController(ProductsContext context)
    {
        _context = context;
    }
    
    //localhost:5299/api/products
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _context.Products!
                            .Where(x => x.IsActive)
                            .Select(x =>  ProductToDTO(x)).ToListAsync();
        return Ok(products);
    }

    //localhost:5299/api/products/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int? id)
    {
        if(id == null){
            return NotFound();
        }
        var product = await _context
                            .Products!
                            .Select(x => ProductToDTO(x))
                            .FirstOrDefaultAsync(x => x.ProductId == id);

        if(product == null){
            return NotFound();
        }
        return Ok(product);
    }
    [HttpPost]
    public async Task<IActionResult> CreateProduct(Product product){
    
        await _context.Products!.AddAsync(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id,Product entity){
    
        if(id != entity.ProductId){
            return BadRequest();
        }
        var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id);

        if(product == null){
            return NotFound();
        }

        product.ProductName = entity.ProductName;
        product.Price = entity.Price;
        product.IsActive = entity.IsActive;

        try{
            await _context.SaveChangesAsync();
        }
        catch(Exception){
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> ProductDelete(int? id){

        if(id == null){
            return NotFound();
        }
        var product = await _context.Products.FirstOrDefaultAsync(x => x.ProductId == id);

        if(product == null){
            return NotFound();
        }

        _context.Products.Remove(product);
        try {
            await _context.SaveChangesAsync();
        }
        catch(Exception){
            return NotFound();
        }

        return NoContent();
    
    }

    private static ProductDTO ProductToDTO(Product product) => new()
    {
        ProductId = product.ProductId,
        ProductName = product.ProductName,
        Price = product.Price
    };

}