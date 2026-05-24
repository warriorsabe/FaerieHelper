local CoriolisStateControlTrigger = {}

CoriolisStateControlTrigger.name = "FaerieHelper/CoriolisStateControlTrigger"

CoriolisStateControlTrigger.placements = {
  name = "default",

  data = {
    newStates = "0,2,3,5,9",
    blacklistMode = false,
    resetOnExit = true,
    flag = ""
  }
}

CoriolisStateControlTrigger.fieldInformation = {
    
    newStates = {
       fieldType = "list",
       elementOptions = {
           fieldType = "integer",
           editable = true
       }, 
       elementDefault = -1,
       elementSeparator = ","
    }
}

return CoriolisStateControlTrigger