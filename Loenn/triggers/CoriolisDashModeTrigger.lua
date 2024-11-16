local CoriolisDashModeTrigger = {}

-- the name must match whatever is inside CustomEntity in c#
CoriolisDashModeTrigger.name = "FaerieHelper/CoriolisDashModeTrigger"

CoriolisDashModeTrigger.placements = {
  -- the name of your placement - we can pick whatever
  name = "default",

  -- the actual data of the placement: this is what goes inside EntityData in c#
  data = { 
    resetOnExit = true,
    newDashMode = "Never"
  }
}
CoriolisDashModeTrigger.fieldInformation = {
    flag = {
        fieldType = "string"   
    },
    newDashMode = {
        options = {
            "Never",
            "OnlyInAir",
            "Always"
        }
    }
}
-- we're ready - give the controller to lönn
return CoriolisDashModeTrigger