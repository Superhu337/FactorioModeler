using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

/// <summary>
/// Представляет предмет (item) в Factorio
/// </summary>
/// 

namespace FactorioModeler.Models
{
    public class FactorioItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("localised_name")]
        public JsonElement LocalisedName { get; set; }

        [JsonPropertyName("localised_description")]
        public JsonElement LocalisedDescription { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("stack_size")]
        public int StackSize { get; set; }

        [JsonPropertyName("fuel_value")]
        public double FuelValue { get; set; }

        [JsonPropertyName("fuel_category")]
        public string FuelCategory { get; set; }

        [JsonPropertyName("subgroup")]
        public string Subgroup { get; set; }

        [JsonPropertyName("order")]
        public string Order { get; set; }

        [JsonPropertyName("flags")]
        public Dictionary<string, object> Flags { get; set; }

        public override string ToString() => $"{LocalisedName} ({Name})";
    }

    /// <summary>
    /// Ингредиент для рецепта
    /// </summary>
    public class RecipeIngredient
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("catalyst_amount")]
        public int CatalystAmount { get; set; }

        public override string ToString() => $"{Amount}x {Name}";
    }

    /// <summary>
    /// Продукт из рецепта
    /// </summary>
    public class RecipeProduct
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("amount_min")]
        public double AmountMin { get; set; }

        [JsonPropertyName("amount_max")]
        public double AmountMax { get; set; }

        [JsonPropertyName("probability")]
        public double Probability { get; set; }

        public override string ToString() =>
            $"{Amount}{(Probability < 1 ? $" ({Probability:P0})" : "")} {Name}";
    }

    /// <summary>
    /// Рецепт в Factorio
    /// </summary>
    public class FactorioRecipe
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("localised_name")]
        public JsonElement LocalisedName { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("energy_required")]
        public double EnergyRequired { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("hidden")]
        public bool Hidden { get; set; }

        [JsonPropertyName("ingredients")]
        public List<RecipeIngredient> Ingredients { get; set; }

        [JsonPropertyName("products")]
        public List<RecipeProduct> Products { get; set; }

        [JsonPropertyName("subgroup")]
        public string Subgroup { get; set; }

        /// <summary>
        /// Получить выход продуктов в секунду
        /// </summary>
        public double GetProductionRate() => 1.0 / EnergyRequired;

        public override string ToString() => $"{LocalisedName} ({EnergyRequired:F1}s)";
    }

    /// <summary>
    /// Машина (станок, печь, и т.д.) в Factorio
    /// </summary>
    public class FactorioMachine
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("localised_name")]
        public JsonElement LocalisedName { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("crafting_speed")]
        public double CraftingSpeed { get; set; }

        [JsonPropertyName("ingredient_count")]
        public int IngredientCount { get; set; }

        [JsonConverter(typeof(EmptyObjectToListConverter<string>))]
        [JsonPropertyName("crafting_categories")]
        public List<string> CraftingCategories { get; set; } = new();


        [JsonPropertyName("pollution")]
        public double Pollution { get; set; }

        [JsonPropertyName("max_health")]
        public int MaxHealth { get; set; }

        [JsonPropertyName("subgroup")]
        public string Subgroup { get; set; }

        public override string ToString() =>
            $"{LocalisedName} ({CraftingSpeed}x, {CraftingCategories?.Count ?? 0} categories)";
    }

    /// <summary>
    /// Эффект от исследования
    /// </summary>
    public class TechnologyEffect
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("recipe")]
        public string Recipe { get; set; }

        public override string ToString() =>
            $"{Type}: {Recipe}";
    }

    /// <summary>
    /// Исследование (технология) в Factorio
    /// </summary>
    public class FactorioTechnology
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("localised_name")]
        public JsonElement LocalisedName { get; set; }

        [JsonPropertyName("localised_description")]
        public JsonElement LocalisedDescription { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("hidden")]
        public bool Hidden { get; set; }

        [JsonPropertyName("upgrade")]
        public bool Upgrade { get; set; }

        [JsonConverter(typeof(EmptyObjectToListConverter<string>))]
        [JsonPropertyName("prerequisites")]
        public List<string> Prerequisites { get; set; } = new();

        [JsonConverter(typeof(EmptyObjectToListConverter<TechnologyEffect>))]
        [JsonPropertyName("effects")]
        public List<TechnologyEffect> Effects { get; set; } = new();

        [JsonPropertyName("order")]
        public string Order { get; set; }

        public override string ToString() =>
            $"{LocalisedName} (prereqs: {Prerequisites?.Count ?? 0})";
    }

    public class IconPathMap
    {
        [JsonPropertyName("items")]
        public Dictionary<string, string> Items { get; set; }

        [JsonPropertyName("recipes")]
        public Dictionary<string, string> Recipes { get; set; }
    }

    public class EmptyObjectToListConverter<T> : JsonConverter<List<T>>
    {
        public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                // Пропускаем пустой объект {}
                reader.Skip();
                return new List<T>();
            }

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                return JsonSerializer.Deserialize<List<T>>(ref reader, options);
            }

            throw new JsonException($"Unexpected token type: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}