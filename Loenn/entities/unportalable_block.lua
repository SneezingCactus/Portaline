local fakeTilesHelper = require("helpers.fake_tiles")

local unportalableBlock = {}

unportalableBlock.name = "Portaline/UnportalableBlock"
unportalableBlock.depth = -13000
unportalableBlock.placements = {
    name = "unportalable_block",
    data = {
        tileType = "3",
        width = 8,
        height = 8
    }
}

unportalableBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tileType", "blendin", "tilesFg", {1.0, 1.0, 1.0, 1.0})
unportalableBlock.fieldInformation = fakeTilesHelper.getFieldInformation("tileType")

return unportalableBlock