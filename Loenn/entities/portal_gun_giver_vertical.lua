local fakeTilesHelper = require("helpers.fake_tiles")
local utils = require("utils")
local matrixLib = require("utils.matrix")
local drawableSprite = require("structs.drawable_sprite")
local drawableRectangle = require("structs.drawable_rectangle")
local connectedEntities = require("helpers.connected_entities")
local math = require("math")

local portalGunGiver = {}

local frame = "Portaline/PortalGunGiverVertical"
local depth = -10

portalGunGiver.name = "Portaline/PortalGunGiverVertical"
portalGunGiver.minimumSize = {16, 16}
portalGunGiver.maximumSize = {16, 99999}
portalGunGiver.placements = {
  name = "portal_gun_giver_vertical",
  data = {
      width = 16,
      height = 16,
      enableGun = true
  }
}

portalGunGiver.sprite = function (room, entity)
    local sprites = {}

    entity.width = math.min(entity.width, 16)

    local color, colorTrans

    if entity.enableGun then
      color = "#00ff1b"
      colorTrans = "#00ff1b88"
    else
      color = "#ff5151"
      colorTrans = "#ff515188"
    end

    table.insert(sprites, drawableRectangle.fromRectangle("fill", entity.x - 1, entity.y, 18, entity.height, colorTrans, colorTrans))
    table.insert(sprites, drawableSprite.fromTexture("Portaline/PortalGunGiver/edge", entity):addPosition(8, 4))
    table.insert(sprites, drawableSprite.fromTexture("Portaline/PortalGunGiver/edge", entity):addPosition(8, entity.height - 4))
    sprites[3].rotation = math.pi

    table.insert(sprites, drawableSprite.fromTexture("Portaline/PortalGunGiver/symbol", entity):addPosition(entity.width / 2, entity.height / 2))
    sprites[4]:setColor(color)

    return sprites
end

return portalGunGiver