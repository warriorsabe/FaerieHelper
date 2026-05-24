local CoriolisBooleanTrigger = {}

CoriolisBooleanTrigger.name = "FaerieHelper/CoriolisBooleanTrigger"

CoriolisBooleanTrigger.placements = {
  name = "default",

  data = { 
    newValue = true,
    resetOnExit = true,
    targetValue = "AffectHoldables",
    flag = ""
  }
}

CoriolisBooleanTrigger.fieldInformation = {
     targetValue = {
         options = {
             "AffectHoldables",
             "AffectPlayer",
             "GravityLike",
             "DashTechProtection"
         }
     }
}
return CoriolisBooleanTrigger