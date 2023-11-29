local fakeTilesHelper = require("helpers.fake_tiles")
local utils = require("utils")
local matrixLib = require("utils.matrix")
local drawableSprite = require("structs.drawable_sprite")
local drawableRectangle = require("structs.drawable_rectangle")
local connectedEntities = require("helpers.connected_entities")
local math = require("math")

local portalGunGiver = {}

local frame = "Portaline/PortalGunGiverHorizontal"
local depth = -10

portalGunGiver.name = "Portaline/PortalGunGiverHorizontal"
portalGunGiver.minimumSize = {16, 16}
portalGunGiver.maximumSize = {99999, 16}
portalGunGiver.placements = {
  name = "portal_gun_giver_horizontal",
  data = {
      width = 16,
      height = 16,
      enableGun = true
  }
}

portalGunGiver.sprite = function (room, entity)
    local sprites = {}

    entity.height = math.min(entity.height, 16)

    local color, colorTrans

    if entity.enableGun then
      color = "#00ff1b"
      colorTrans = "#00ff1b88"
    else
      color = "#ff5151"
      colorTrans = "#ff515188"
    end

    table.insert(sprites, drawableRectangle.fromRectangle("fill", entity.x, entity.y - 1, entity.width, 18, colorTrans, colorTrans))
    table.insert(sprites, drawableSprite.fromTexture("Portaline/PortalGunGiver/edge", entity):addPosition(4, 8))
    table.insert(sprites, drawableSprite.fromTexture("Portaline/PortalGunGiver/edge", entity):addPosition(entity.width - 4, 8))
    sprites[2].rotation = math.pi*1.5
    sprites[3].rotation = math.pi*0.5

    table.insert(sprites, drawableSprite.fromTexture("Portaline/PortalGunGiver/symbol", entity):addPosition(entity.width / 2, entity.height / 2))
    sprites[4]:setColor(color)

    return sprites
end

return portalGunGiver