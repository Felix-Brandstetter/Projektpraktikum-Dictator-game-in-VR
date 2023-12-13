import pandas as pd

df = pd.DataFrame([[1,2,3,4],[5,6,7,8]],index=["a","b"],columns=["A","B","C","D"])

print(df.to_string()) 
a = df.loc["a",df.loc["a"]==1].keys()[0]
print()
print(a)