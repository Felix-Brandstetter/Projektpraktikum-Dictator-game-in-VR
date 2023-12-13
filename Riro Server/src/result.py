import json
from pickle import FALSE
class Result:
    def __init__(self,id) -> None:
        self.id =id
        self.round1Sent = 1
        self.round1Received = 1
        self.round2Sent = 1
        self.round2Received = 1
        self.round3Sent = 1
        self.round3Received = 1
        self.round1MyDecision = True
        self.round2MyDecision = False
        self.round3MyDecision = True
    
    def toJSON(self):
            return json.dumps(self, default=lambda o: o.__dict__, sort_keys=False)
    
    def __str__(self):
        return str(vars(self))