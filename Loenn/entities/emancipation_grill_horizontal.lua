local fakeTilesHelper = require("helpers.fake_tiles")
local utils = require("utils")
local matrixLib = require("utils.matrix")
local drawableSprite = require("structs.drawable_sprite")
local drawableRectangle = require("structs.drawable_rectangle")
local connectedEntities = require("helpers.connected_entities")
local math = require("math")

local emancipationGrill = {}

local frame = "Portaline/EmancipationGrillHorizontal"
local depth = -10

emancipationGrill.name = "Portaline/EmancipationGrillHorizontal"
emancipationGrill.minimumSize = {16, 16}
emancipationGrill.maximumSize = {99999, 16}
emancipationGrill.placements = {
  name = "emancipation_grill_horizontal",
  data = {
      width = 8,
      height = 8
  }
}

emancipationGrill.sprite = function (room, entity)
    local sprites = {}

    local width = entity.width
    local height = entity.height
    
    table.insert(sprites, drawableRectangle.fromRectangle("fill", entity.x, entity.y + 1, entity.width, 14, "#00bfff88", "#00bfff88"))

    table.insert(sprites, drawableSprite.fromTexture("Portaline/EmancipationGrill", entity):addPosition(4, 8))
    table.insert(sprites, drawableSprite.fromTexture("Portaline/EmancipationGrill", entity):addPosition(width - 4, 8))

    sprites[2].rotation = math.pi * 1.5
    sprites[3].rotation = math.pi * 0.5

    return sprites
end

return emancipationGrill