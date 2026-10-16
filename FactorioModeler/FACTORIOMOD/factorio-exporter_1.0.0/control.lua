-- Factorio Data Exporter Mod
-- Экспортирует всю информацию о предметах, рецептах, машинах и исследованиях в JSON
-- Команда для запуска: /c remote.call("factorio-exporter", "export_all")

local json = require("json")

-- Создание таблицы с функциями мода
local exporter = {}

-- Функция для получения значения свойства БЕЗОПАСНО (без ошибок)
local function safe_get(obj, key, default)
    local success, result = pcall(function()
        return obj[key]
    end)
    if success then
        return result or default
    else
        return default
    end
end

-- Функция для экспорта всех предметов
function exporter.export_items()
    local items = {}
    
    for name, item in pairs(game.item_prototypes) do
        local item_data = {
            name = name,
            localised_name = safe_get(item, "localised_name", name),
            localised_description = safe_get(item, "localised_description", ""),
            type = safe_get(item, "type", "item"),
            stack_size = safe_get(item, "stack_size", 1),
            fuel_value = safe_get(item, "fuel_value", 0),
            fuel_category = safe_get(item, "fuel_category", nil),
            subgroup = (function()
                local subgroup = safe_get(item, "subgroup", nil)
                if subgroup then
                    return safe_get(subgroup, "name", nil)
                end
                return nil
            end)(),
            order = safe_get(item, "order", ""),
            flags = safe_get(item, "flags", {})
        }
        
        table.insert(items, item_data)
    end
    
    return items
end

-- Функция для экспорта всех рецептов
function exporter.export_recipes()
    local recipes = {}
    
    for name, recipe in pairs(game.recipe_prototypes) do
        local ingredients = {}
        local recipe_ingredients = safe_get(recipe, "ingredients", {})
        for _, ingredient in pairs(recipe_ingredients) do
            table.insert(ingredients, {
                type = safe_get(ingredient, "type", "item"),
                name = safe_get(ingredient, "name", ""),
                amount = safe_get(ingredient, "amount", 1),
                catalyst_amount = safe_get(ingredient, "catalyst_amount", 0)
            })
        end
        
        local products = {}
        local recipe_products = safe_get(recipe, "products", {})
        for _, product in pairs(recipe_products) do
            table.insert(products, {
                type = safe_get(product, "type", "item"),
                name = safe_get(product, "name", ""),
                amount = safe_get(product, "amount", 1),
                amount_min = safe_get(product, "amount_min", 0),
                amount_max = safe_get(product, "amount_max", 0),
                probability = safe_get(product, "probability", 1)
            })
        end
        
        -- Получаем время рецепта БЕЗОПАСНО
        local energy_time = safe_get(recipe, "energy", 0)
        
        local recipe_data = {
            name = name,
            localised_name = safe_get(recipe, "localised_name", name),
            category = safe_get(recipe, "category", "crafting"),
            energy_required = energy_time,
            enabled = safe_get(recipe, "enabled", false),
            hidden = safe_get(recipe, "hidden", false),
            ingredients = ingredients,
            products = products,
            subgroup = (function()
                local subgroup = safe_get(recipe, "subgroup", nil)
                if subgroup then
                    return safe_get(subgroup, "name", nil)
                end
                return nil
            end)()
        }
        
        table.insert(recipes, recipe_data)
    end
    
    return recipes
end

-- Функция для экспорта всех машин (сущностей)
function exporter.export_machines()
    local machines = {}
    local machine_types = {
        "assembling-machine",
        "furnace",
        "mining-drill",
        "pumpjack",
        "generator",
        "reactor",
        "lab"
    }
    
    for entity_name, entity in pairs(game.entity_prototypes) do
        local is_machine = false
        for _, machine_type in pairs(machine_types) do
            if entity.type == machine_type then
                is_machine = true
                break
            end
        end
        
        if is_machine then
            local crafting_categories = {}
            local categories = safe_get(entity, "crafting_categories", nil)
            if categories then
                for category in pairs(categories) do
                    table.insert(crafting_categories, category)
                end
            end
            
            local energy_source_data = nil
            local energy_source = safe_get(entity, "energy_source", nil)
            if energy_source then
                energy_source_data = {
                    type = safe_get(energy_source, "type", "unknown")
                }
                local efficiency = safe_get(energy_source, "efficiency", nil)
                if efficiency then
                    energy_source_data.efficiency = efficiency
                end
            end
            
            local entity_data = {
                name = entity_name,
                localised_name = safe_get(entity, "localised_name", entity_name),
                type = entity.type,
                energy_source = energy_source_data,
                crafting_speed = safe_get(entity, "crafting_speed", 1),
                ingredient_count = safe_get(entity, "ingredient_count", 0),
                crafting_categories = crafting_categories,
                pollution = safe_get(entity, "pollution", 0),
                max_health = safe_get(entity, "max_health", 0),
                subgroup = (function()
                    local subgroup = safe_get(entity, "subgroup", nil)
                    if subgroup then
                        return safe_get(subgroup, "name", nil)
                    end
                    return nil
                end)()
            }
            
            table.insert(machines, entity_data)
        end
    end
    
    return machines
end

-- Функция для экспорта всех исследований (технологий)
function exporter.export_technologies()
    local technologies = {}
    
    for tech_name, tech in pairs(game.technology_prototypes) do
        local prerequisites = {}
        local tech_prerequisites = safe_get(tech, "prerequisites", {})
        for _, prerequisite in pairs(tech_prerequisites) do
            table.insert(prerequisites, safe_get(prerequisite, "name", ""))
        end
        
        local effects = {}
        local tech_effects = safe_get(tech, "effects", {})
        for _, effect in pairs(tech_effects) do
            local effect_data = {
                type = safe_get(effect, "type", "")
            }
            local recipe = safe_get(effect, "recipe", nil)
            if recipe then
                effect_data.recipe = recipe
            end
            local technology = safe_get(effect, "technology", nil)
            if technology then
                effect_data.technology = technology
            end
            table.insert(effects, effect_data)
        end
        
        local tech_data = {
            name = tech_name,
            localised_name = safe_get(tech, "localised_name", tech_name),
            localised_description = safe_get(tech, "localised_description", ""),
            enabled = safe_get(tech, "enabled", false),
            hidden = safe_get(tech, "hidden", false),
            upgrade = safe_get(tech, "upgrade", false),
            prerequisites = prerequisites,
            effects = effects,
            order = safe_get(tech, "order", "")
        }
        
        table.insert(technologies, tech_data)
    end
    
    return technologies
end

-- Функция для получения пути к иконке БЕЗОПАСНО
function exporter.get_icon_path(prototype)
    local icon = safe_get(prototype, "icon", nil)
    if icon then
        return icon
    end
    
    local icons = safe_get(prototype, "icons", nil)
    if icons then
        for i = 1, #icons do
            local icon_data = icons[i]
            if icon_data then
                local icon_path = safe_get(icon_data, "icon", nil)
                if icon_path then
                    return icon_path
                end
            end
        end
    end
    return nil
end

-- Функция для копирования иконок
function exporter.copy_icons(output_dir)
    local icon_mapping = {
        items = {},
        recipes = {}
    }
    
    -- Копируем иконки предметов
    for name, item in pairs(game.item_prototypes) do
        local icon_path = exporter.get_icon_path(item)
        if icon_path then
            icon_mapping.items[name] = icon_path
        end
    end
    
    -- Копируем иконки рецептов
    for name, recipe in pairs(game.recipe_prototypes) do
        local icon_path = exporter.get_icon_path(recipe)
        if icon_path then
            icon_mapping.recipes[name] = icon_path
        end
    end
    
    return icon_mapping
end

-- Основная функция экспорта
function exporter.export_all()
    game.print("Starting export...")
    local success, error_msg = pcall(function()
        local export_data = {
            timestamp = game.tick,
            factorio_version = "1.1.1",
            items = exporter.export_items(),
            recipes = exporter.export_recipes(),
            machines = exporter.export_machines(),
            technologies = exporter.export_technologies(),
            icons = exporter.copy_icons("icons")
        }
        
        -- Сохраняем в JSON файлы
        -- write_file (path, contents, [append], [relative_to])
        game.write_file(
            "exporter/items.json",
            json.encode(export_data.items)
        )
        
        game.write_file(
            "exporter/recipes.json",
            json.encode(export_data.recipes)
        )
        
        game.write_file(
            "exporter/machines.json",
            json.encode(export_data.machines)
        )
        
        game.write_file(
            "exporter/technologies.json",
            json.encode(export_data.technologies)
        )
        
        game.write_file(
            "exporter/icons.json",
            json.encode(export_data.icons)
        )
        
        game.print("✓ Data exported successfully!")
        game.print("Files saved to: script-output/exporter/")
        game.print("")
        game.print("5 JSON files created:")
        game.print("  • items.json - all items")
        game.print("  • recipes.json - all recipes")
        game.print("  • machines.json - all machines")
        game.print("  • technologies.json - all technologies")
        game.print("  • icons.json - icon paths")
        return true
    end)
    
    if not success then
        game.print("✗ Export failed: " .. error_msg)
        game.print("Error details: " .. tostring(error_msg))
        return false
    end
end

-- Регистрируем функции для вызова через консоль/remote call
commands.add_command("export-factorio-data", "Export all Factorio data to JSON", function()
    exporter.export_all()
end)

-- Через remote.call
remote.add_interface("factorio-exporter", {
    export_all = function()
        return exporter.export_all()
    end,
    export_items = function()
        return exporter.export_items()
    end,
    export_recipes = function()
        return exporter.export_recipes()
    end,
    export_machines = function()
        return exporter.export_machines()
    end,
    export_technologies = function()
        return exporter.export_technologies()
    end
})

-- ============================================================================
-- АВТОМАТИЧЕСКИЙ ЭКСПОРТ при создании/загрузке игры (БЕЗ команды!)
-- ============================================================================

-- Инициализация при создании нового мира или загрузке сохранения
script.on_init(function()
    game.print("[Factorio Exporter] 🎮 Экспортирую данные Factorio...")
    exporter.export_all()
    game.print("[Factorio Exporter] ✓ Готово!")
end)