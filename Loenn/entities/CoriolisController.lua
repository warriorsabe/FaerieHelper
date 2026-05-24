local CoriolisController = {}

CoriolisController.name = "FaerieHelper/CoriolisController"
CoriolisController.depth = -100000
CoriolisController.texture = "controllers/FaerieHelper/CoriolisController"

CoriolisController.placements = {
    name = "default",
    
    data = {
        coriolisFlag = "",  
        strength = 140,
        blacklistMode = false,
        affectPlayer = true,
        affectHoldables = true,
        gravityLike = true,
        dashTechProtection = true,
        
        legacyBehavior = false,
        
        affectedStates = "0,2,3,5,9"
    }
}

CoriolisController.fieldInformation = {
    affectedStates = {
       fieldType = "list",
       elementOptions = {
           fieldType = "integer",
           editable = true
       }, 
       elementDefault = -1,
       elementSeparator = ","
    },
    affectDashMode = {
        options = {
            "Never",
            "OnlyInAir",
            "Always"
        }
    }
}

CoriolisController.ignoredFields = {
    "legacyBehavior"   
}

return CoriolisController