local CoriolisStrengthTrigger = {}

-- the name must match whatever is inside CustomEntity in c#
CoriolisStrengthTrigger.name = "FaerieHelper/CoriolisStrengthTrigger"

CoriolisStrengthTrigger.placements = {
  -- the name of your placement - we can pick whatever
  name = "default",

  -- the actual data of the placement: this is what goes inside EntityData in c#
  data = { 
    newStrength = -2.5,
    resetOnExit = true
  }
}
CoriolisStrengthTrigger.fieldInformation = {
    flag = {
        fieldType = "string"   
    }
}
-- we're ready - give the controller to lönn
return CoriolisStrengthTrigger