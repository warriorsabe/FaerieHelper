local CoriolisBooleanTrigger = {}

-- the name must match whatever is inside CustomEntity in c#
CoriolisBooleanTrigger.name = "FaerieHelper/CoriolisBooleanTrigger"

CoriolisBooleanTrigger.placements = {
  -- the name of your placement - we can pick whatever
  name = "default",

  -- the actual data of the placement: this is what goes inside EntityData in c#
  data = { 
    newValue = true,
    resetOnExit = true,
    targetValue = "AffectHoldables"
  }
}
CoriolisBooleanTrigger.fieldInformation = {
    flag = {
        fieldType = "string"   
    },
     targetValue = {
         options = {
             "AffectHoldables",
             "AffectPlayer",
             "AffectHorizontal",
             "AffectVertical",
             "GravityLike"
         }
     }
}
-- we're ready - give the controller to lönn
return CoriolisBooleanTrigger