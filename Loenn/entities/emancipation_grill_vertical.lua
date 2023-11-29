local fakeTilesHelper = require("helpers.fake_tiles")
local utils = require("utils")
local matrixLib = require("utils.matrix")
local drawableSprite = require("structs.drawable_sprite")
local drawableRectangle = require("structs.drawable_rectangle")
local connectedEntities = require("helpers.connected_entities")
local math = require("math")

local emancipationGrill = {}

local frame = "Portaline/EmancipationGrillVertical"
local depth = -10

emancipationGrill.name = "Portaline/EmancipationGrillVertical"
emancipationGrill.minimumSize = {16, 16}
emancipationGrill.maximumSize = {16, 99999}
emancipationGrill.placements = {
  name = "emancipation_grill_vertical",
  data = {
      width = 8,
      height = 8
  }
}

emancipationGrill.sprite = function (room, entity)
    local sprites = {}

    local width = entity.width
    local height = entity.height
    
    table.insert(sprites, drawableRectangle.fromRectangle("fill", entity.x + 1, entity.y, 14, entity.height, "#00bfff88", "#00bfff88"))

    table.insert(sprites, drawableSprite.fromTexture("Portaline/EmancipationGrill", entity):addPosition(8, 4))
    table.insert(sprites, drawableSprite.fromTexture("Portaline/EmancipationGrill", entity):addPosition(8, height - 4))

    sprites[3].rotation = math.pi

    return sprites
end

return emancipationGrill