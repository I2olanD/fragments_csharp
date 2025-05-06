using System.Text;

public static class MaterialDirectAccessor
{
    /// <summary>
    /// Attempts to get material information using multiple strategies
    /// </summary>
    public static MaterialInfo GetMaterial(Model model, int index)
    {
        if (model.Meshes == null || index >= model.Meshes.Value.MaterialsLength)
        {
            return CreateDefaultMaterial("Material index out of range");
        }

        // Strategy 1: Standard accessor
        try
        {
            var material = model.Meshes.Value.Materials(index);
            if (material.HasValue)
            {
                return new MaterialInfo
                {
                    R = material.Value.R,
                    G = material.Value.G,
                    B = material.Value.B,
                    A = material.Value.A,
                    RenderedFaces = material.Value.RenderedFaces.ToString(),
                    Stroke = material.Value.Stroke.ToString(),
                    IsValid = true,
                    AccessMethod = "Standard",
                    MaterialIndex = index
                };
            }
        }
        catch (Exception ex)
        {
            // Record the exception for diagnostics
            var materialInfo = CreateDefaultMaterial($"Standard access failed: {ex.Message}");
            materialInfo.MaterialIndex = index;
            materialInfo.ExceptionType = ex.GetType().Name;
            materialInfo.ErrorDetails = ex.ToString();
            return materialInfo;
        }

        // Strategy 2: Try to extract using other accessible materials as reference
        try 
        {
            // Find a working material index
            int referenceIndex = -1;
            for (int i = 0; i < model.Meshes.Value.MaterialsLength; i++)
            {
                if (i == index) continue; // Skip the current index
                
                try
                {
                    var material = model.Meshes.Value.Materials(i);
                    if (material.HasValue)
                    {
                        referenceIndex = i;
                        break;
                    }
                }
                catch
                {
                    // This material also failed, continue to next
                }
            }
            
            if (referenceIndex >= 0)
            {
                // We found a reference material that works
                // Use it to diagnose our target material
                var refMaterial = model.Meshes.Value.Materials(referenceIndex);
                
                return new MaterialInfo
                {
                    // Use a distinct diagnostic color
                    R = 128,
                    G = 0,
                    B = 128,
                    A = 255,
                    RenderedFaces = "UNKNOWN",
                    Stroke = "UNKNOWN",
                    IsValid = false,
                    AccessMethod = $"Reference comparison (index {referenceIndex})",
                    MaterialIndex = index,
                    ReferenceIndex = referenceIndex,
                    RefR = refMaterial.Value.R,
                    RefG = refMaterial.Value.G,
                    RefB = refMaterial.Value.B,
                    RefA = refMaterial.Value.A,
                    ErrorDetails = "Could not access directly"
                };
            }
        }
        catch (Exception ex)
        {
            // Reference strategy failed
        }

        // All strategies failed
        var defaultMaterial = CreateDefaultMaterial("All access strategies failed");
        defaultMaterial.MaterialIndex = index;
        return defaultMaterial;
    }

    private static MaterialInfo CreateDefaultMaterial(string error)
    {
        return new MaterialInfo
        {
            R = 255,
            G = 0,
            B = 255, // Magenta for error indication
            A = 255,
            RenderedFaces = "UNKNOWN",
            Stroke = "UNKNOWN",
            IsValid = false,
            AccessMethod = "Default",
            ErrorDetails = error
        };
    }
    
    /// <summary>
    /// Analyzes the structure of the materials array to help diagnose access issues
    /// </summary>
    public static string AnalyzeMaterialStructure(Model model)
    {
        if (model.Meshes == null)
        {
            return "No Meshes found in model";
        }
        
        var builder = new StringBuilder();
        builder.AppendLine("Material Structure Analysis");
        builder.AppendLine("==========================");
        
        var materialsLength = model.Meshes.Value.MaterialsLength;
        builder.AppendLine($"Total Materials: {materialsLength}");
        
        // Try to access a sample of materials to determine access pattern
        var accessResults = new Dictionary<int, bool>();
        for (int i = 0; i < Math.Min(100, materialsLength); i++)
        {
            try
            {
                var material = model.Meshes.Value.Materials(i);
                accessResults[i] = material.HasValue;
            }
            catch
            {
                accessResults[i] = false;
            }
        }
        
        var accessible = accessResults.Count(r => r.Value);
        var inaccessible = accessResults.Count - accessible;
        
        builder.AppendLine($"Sampled Materials: {accessResults.Count}");
        builder.AppendLine($"Accessible: {accessible}, Inaccessible: {inaccessible}");
        
        // Look for patterns in accessibility
        if (inaccessible > 0)
        {
            builder.AppendLine("\nAccessibility Pattern:");
            
            // Check for potential stride patterns (e.g., every Nth material accessible)
            var stridePatterns = new Dictionary<int, int>();
            for (int stride = 2; stride <= 10; stride++)
            {
                int matches = 0;
                for (int i = 0; i < accessResults.Count; i++)
                {
                    if (i % stride == 0 && accessResults[i])
                    {
                        matches++;
                    }
                }
                if (matches > 0)
                {
                    stridePatterns[stride] = matches;
                }
            }
            
            if (stridePatterns.Count > 0)
            {
                var bestStride = stridePatterns.OrderByDescending(p => p.Value).First();
                builder.AppendLine($"  Possible stride pattern detected: Every {bestStride.Key}th material accessible ({bestStride.Value} matches)");
            }
            
            // Check contiguous blocks pattern
            int currentBlock = 0;
            int blockLength = 0;
            bool inBlock = false;
            var blocks = new List<(int start, int length, bool accessible)>();
            
            for (int i = 0; i < accessResults.Count; i++)
            {
                if (i == 0 || accessResults[i] != accessResults[i-1])
                {
                    if (blockLength > 0)
                    {
                        blocks.Add((currentBlock, blockLength, inBlock));
                    }
                    currentBlock = i;
                    blockLength = 1;
                    inBlock = accessResults[i];
                }
                else
                {
                    blockLength++;
                }
            }
            
            if (blockLength > 0)
            {
                blocks.Add((currentBlock, blockLength, inBlock));
            }
            
            if (blocks.Count <= 5)
            {
                builder.AppendLine("  Contiguous accessibility blocks:");
                foreach (var block in blocks)
                {
                    builder.AppendLine($"    Indices {block.start}-{block.start + block.length - 1}: {(block.accessible ? "Accessible" : "Inaccessible")}");
                }
            }
            else
            {
                builder.AppendLine($"  Complex accessibility pattern with {blocks.Count} blocks");
                builder.AppendLine($"  First few blocks:");
                foreach (var block in blocks.Take(3))
                {
                    builder.AppendLine($"    Indices {block.start}-{block.start + block.length - 1}: {(block.accessible ? "Accessible" : "Inaccessible")}");
                }
            }
        }
        
        // Try to examine the material class structure
        builder.AppendLine("\nMaterial Class Structure:");
        try
        {
            var materialType = typeof(Material);
            var fields = materialType.GetFields();
            builder.AppendLine($"  Material Fields: {string.Join(", ", fields.Select(f => f.Name))}");
            
            var properties = materialType.GetProperties();
            builder.AppendLine($"  Material Properties: {string.Join(", ", properties.Select(p => p.Name))}");
            
            // Look at model meshes structure
            if (model.Meshes.HasValue)
            {
                var meshesType = model.Meshes.Value.GetType();
                builder.AppendLine($"\nMeshes Class: {meshesType.Name}");
                
                var materialsProp = meshesType.GetProperties()
                    .FirstOrDefault(p => p.Name.Contains("Material"));
                
                if (materialsProp != null)
                {
                    builder.AppendLine($"  Materials Property: {materialsProp.Name} ({materialsProp.PropertyType})");
                }
                else
                {
                    builder.AppendLine("  No Materials property found directly");
                }
                
                // Check for nested buffer property
                var bufferProps = meshesType.GetProperties()
                    .Where(p => p.Name.Contains("Buffer") || p.PropertyType.Name.Contains("Buffer"))
                    .ToList();
                
                if (bufferProps.Any())
                {
                    builder.AppendLine($"  Buffer Properties: {string.Join(", ", bufferProps.Select(p => p.Name))}");
                }
            }
        }
        catch (Exception ex)
        {
            builder.AppendLine($"  Error analyzing Material class: {ex.Message}");
        }
        
        return builder.ToString();
    }
}

public class MaterialInfo
{
    // Basic properties
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }
    public string RenderedFaces { get; set; } = string.Empty;
    public string Stroke { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    
    // Diagnostic properties
    public string AccessMethod { get; set; } = string.Empty;
    public string ErrorDetails { get; set; } = string.Empty;
    public string ExceptionType { get; set; } = string.Empty;
    public int MaterialIndex { get; set; } = -1;
    public int? ReferenceIndex { get; set; }
    
    // Reference material properties (if using a reference)
    public byte? RefR { get; set; }
    public byte? RefG { get; set; }
    public byte? RefB { get; set; }
    public byte? RefA { get; set; }
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Material #{MaterialIndex}:");
        sb.AppendLine($"  Color: rgba({R},{G},{B},{A}) - Hex: {ToHexColor()}");
        sb.AppendLine($"  Faces: {RenderedFaces}, Stroke: {Stroke}");
        sb.AppendLine($"  Valid: {IsValid}, Access Method: {AccessMethod}");
        
        if (!string.IsNullOrEmpty(ErrorDetails))
            sb.AppendLine($"  Error: {ErrorDetails}");
            
        if (!string.IsNullOrEmpty(ExceptionType))
            sb.AppendLine($"  Exception: {ExceptionType}");
            
        if (ReferenceIndex.HasValue)
        {
            sb.AppendLine($"  Reference Index: {ReferenceIndex}");
            if (RefR.HasValue && RefG.HasValue && RefB.HasValue && RefA.HasValue)
            {
                sb.AppendLine($"  Reference Color: rgba({RefR},{RefG},{RefB},{RefA})");
            }
        }
            
        return sb.ToString();
    }
    
    public string ToHexColor()
    {
        return $"#{R:X2}{G:X2}{B:X2}";
    }
}