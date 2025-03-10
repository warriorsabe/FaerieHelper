local CoriolisController = {}

-- the name must match whatever is inside CustomEntity in c#
CoriolisController.name = "FaerieHelper/CoriolisController"
CoriolisController.depth = -100
CoriolisController.texture = "controllers/FaerieHelper/CoriolisController"

CoriolisController.placements = {
  -- the name of your placement - we can pick whatever
  name = "default",

  -- the actual data of the placement: this is what goes inside EntityData in c#
  data = {
    coriolisFlag = "",  
    strength = 2.5,
    affectDreamBlocks = true,
    affectRedBoosters = true,
    affectFeather = false,
    affectPlayer = true,
    affectHoldables = true,
    affectUndefined = false,
    gravityLike = true,
    affectDirectionMode = "Both",
    affectDashMode = "OnlyInAir"
  }
}

CoriolisController.fieldInformation = {
    coriolisFlag = {
        fieldType = "string"   
    },
    affectDashMode = {
        options = {
            "Never",
            "OnlyInAir",
            "Always"
        }
    },
    affectDirectionMode = {
        options = {
         "Horizontal",
         "Vertical",
         "Both"
        }
    }
}

-- we're ready - give the controller to lönn
return CoriolisController