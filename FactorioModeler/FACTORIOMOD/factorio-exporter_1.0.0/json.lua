-- JSON encoding/decoding for Factorio
-- Поддержка базового JSON для экспорта данных

local json = {}

local function escape_string(str)
    local str_str = tostring(str)
    str_str = string.gsub(str_str, '\\', '\\\\')
    str_str = string.gsub(str_str, '"', '\\"')
    str_str = string.gsub(str_str, '\n', '\\n')
    str_str = string.gsub(str_str, '\r', '\\r')
    str_str = string.gsub(str_str, '\t', '\\t')
    str_str = string.gsub(str_str, '\b', '\\b')
    str_str = string.gsub(str_str, '\f', '\\f')
    return str_str
end

local function encode_value(val, seen, indent, current_indent)
    seen = seen or {}
    indent = indent or ""
    current_indent = current_indent or ""
    
    local val_type = type(val)
    
    if val_type == "nil" then
        return "null"
    elseif val_type == "boolean" then
        return val and "true" or "false"
    elseif val_type == "number" then
        if val ~= val then  -- NaN
            return "null"
        elseif val == math.huge or val == -math.huge then
            return "null"
        end
        -- Форматируем числа
        if val == math.floor(val) then
            return tostring(math.floor(val))
        else
            return string.format("%.10g", val)
        end
    elseif val_type == "string" then
        return '"' .. escape_string(val) .. '"'
    elseif val_type == "table" then
        if seen[val] then
            return "{}"  -- Avoid infinite recursion
        end
        seen[val] = true
        
        local is_array = true
        local max_index = 0
        local has_non_numeric_keys = false
        
        for k in pairs(val) do
            if type(k) == "number" then
                if k >= 1 and k == math.floor(k) then
                    max_index = math.max(max_index, k)
                else
                    has_non_numeric_keys = true
                end
            else
                has_non_numeric_keys = true
            end
        end
        
        -- Если есть пропуски в индексах или непорядок, это объект
        if has_non_numeric_keys then
            is_array = false
        else
            for i = 1, max_index do
                if val[i] == nil then
                    is_array = false
                    break
                end
            end
        end
        
        local result
        if is_array and max_index > 0 then
            -- Array format
            result = "["
            for i = 1, max_index do
                if i > 1 then result = result .. "," end
                result = result .. encode_value(val[i], seen, indent, current_indent)
            end
            result = result .. "]"
        else
            -- Object format
            result = "{"
            local first = true
            
            -- Сортируем ключи для консистентности
            local keys = {}
            for k in pairs(val) do
                if type(k) == "string" then
                    table.insert(keys, k)
                end
            end
            table.sort(keys)
            
            for _, k in ipairs(keys) do
                local v = val[k]
                if v ~= nil then
                    if not first then result = result .. "," end
                    result = result .. '"' .. escape_string(k) .. '":' .. encode_value(v, seen, indent, current_indent)
                    first = false
                end
            end
            result = result .. "}"
        end
        
        seen[val] = nil
        return result
    else
        return "null"
    end
end

function json.encode(val)
    return encode_value(val)
end

return json
