local CoriolisDirectionModeTrigger = {}

-- the name must match whatever is inside CustomEntity in c#
CoriolisDirectionModeTrigger.name = "FaerieHelper/CoriolisDirectionModeTrigger"

CoriolisDirectionModeTrigger.placements = {
  -- the name of your placement - we can pick whatever
  name = "default",

  -- the actual data of the placement: this is what goes inside EntityData in c#
  data = { 
    resetOnExit = true,
    newDirectionMode = "Both"
  }
}
CoriolisDirectionModeTrigger.fieldInformation = {
    flag = {
        fieldType = "string"   
    },
    newDirectionMode = {
        options = {
            "Horizontal",
            "Vertical",
            "Both"
        }
    }
}
-- we're ready - give the controller to lönn
return CoriolisDirectionModeTrigger