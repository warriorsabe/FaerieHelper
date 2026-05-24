local fakeTilesHelper = require("helpers.fake_tiles")

local MomentumPreservingDashBlock = {}

MomentumPreservingDashBlock.name = "FaerieHelper/MomentumPreservingDashBlock"
MomentumPreservingDashBlock.depth = 0

MomentumPreservingDashBlock.placements = {
    name = "default",
    
    data = {
        width = 8,
        height = 8,
        tiletype = fakeTilesHelper.getPlacementMaterial('G'),
        blendin = false,
        permanent = false,
        
        duration = 0.15,
        addedSpeed = 30,
        resetCooldown = true,
        breakRedBubbles = false,
        multiplicative = false,
        onBreakFlag = ""
    }
}

MomentumPreservingDashBlock.ignoredFields = {
    "permanent"   
}

MomentumPreservingDashBlock.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", "blendin")
MomentumPreservingDashBlock.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")

return MomentumPreservingDashBlock