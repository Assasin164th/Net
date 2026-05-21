second_per_day = 1*24*60*60
print(second_per_day)
second_per_hour = 1*60*60
print(second_per_hour)
N2 = second_per_day/second_per_hour
N3 = second_per_day//second_per_hour

if N2 == N3:
    input("Значения совпадают")
else:
    input("Значения не совпадают")