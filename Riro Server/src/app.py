import datetime
import string
from flask import Flask, request, jsonify
from flask_cors import CORS
import random
import pandas as pd
import numpy as np
from result import Result

app = Flask(__name__)
CORS(app)

# Global Varibales
df = None
session_id = None
results_list = []
list_of_player_ids =[]


@app.route('/new-session', methods=['GET'])
def new_session():
    global session_id, df, results_list
    if session_id is not None:
        store()

    session_id = random.randint(10000, 99999)
    index_names = ["Session", "Device", "StartTime", "AttentionCheck", "Decision_in_Round1", "Decision_in_Round2",
                   "Decision_in_Round3", "PayOutMoney in Euro", "Health", "Page"]
    df = pd.DataFrame(index=index_names)
    results_list = []
    return str(session_id)


@app.route('/store', methods=['GET'])
def store():
    global session_id, df, results_list
    store()
    return str("True")


@app.route('/register/<phone_id>', methods=['GET'])
def register(phone_id: str):
    global session_id, df
    assert (session_id is not None)
    if len(str(phone_id)) > 3:
        print(df.loc["Device"].values)
        if phone_id not in df.loc["Device"].values:
            pid = None
            while pid is None:
                rnd = str(random.randint(1000, 4999))
                if rnd not in df.columns and rnd not in list_of_player_ids:
                    list_of_player_ids.append(rnd)
                    pid = rnd
            df[pid] = np.nan
            now = datetime.datetime.now()
            df.loc["StartTime", pid] = now.strftime("%Y-%m-%d %H:%M:%S")
            df.loc["Device", pid] = phone_id
            df.loc["Session", pid] = session_id
            print(df)
            return pid
        else:
            #If Phone is registered return corresponding pid
            pid = df.loc["Device",df.loc["Device"]==phone_id].keys()[0]
            return str(pid)
    else:
        return str("False")

@app.route('/delete_player/<pid>', methods=['GET'])
def delete_player(pid: str):
    global df
    assert (session_id is not None)
    if pid not in df.columns:
        return str("False")
    else:
        df.drop(pid, inplace=True, axis=1)
        return str("Player Deleted")


@app.route('/attention-check/<pid>/<attention>', methods=['GET'])
def attention_check(pid: str, attention: str):
    assert (session_id is not None)

    df.loc["AttentionCheck", pid] = attention
    print(df)
    return str("success")


@app.route('/send-dictator-decision/<pid>/<round>/<amount>', methods=['GET'])
def send_dictator_decision(pid: string, round: string, amount: string):
    assert (session_id is not None)
    assert (round in ["1", "2", "3"])
    assert (0 <= int(amount) <= 10)

    print(round)
    df.loc["Decision_in_Round" + round, pid] = amount
    print(df)
    return str("sucess")


@app.route('/check-round-end/<round>', methods=['GET'])
def check_round_end(round):
    assert (session_id is not None)
    assert (round in ["1", "2", "3"])

    print(df)
    if df.loc["Decision_in_Round" + round].isnull().values.any() or df.shape[1] < 4:
        return str("False")
    else:
        return str("True")


@app.route('/results/<pid>', methods=['GET'])
def results(pid):
    global results_list, session_id
    assert (session_id is not None)
    if results_list == []:
        print("Create Results")
        create_Results_Objects()
    for result in results_list:
        print(pid)
        print(result.id)
        if pid == result.id:
            return result.toJSON()
    print("Hier darfst du nicht ankommen")


@app.route('/admin', methods=['GET'])
def admin():
    global session_id
    if session_id == None:
        return "No Session open"
    return df.to_html()


@app.route('/health/<pid>', methods=['GET'])
def update_device_health(pid):
    if df is not None:
        if pid in df.columns:
            df.loc["Health", pid] = datetime.datetime.now()

        if session_id is not None:
            return str(session_id)
    return "0"


@app.route('/health/<pid>/page/<page>', methods=['GET'])
def update_device_page(pid, page):
    if df is not None:
        if pid in df.columns:
            df.loc["Page", pid] = page
            return str("success")

    return str("not found")


# ToDo
# Delete Gerät


def store():
    global df, session_id
    path = ".\data\dataset_session{}.csv".format(str(session_id))
    df.to_csv(str(path), mode='w', index=True, header=True)


def create_Results_Objects():
    global df, results_list
    list_of_pid = df.columns
    id_player1 = list_of_pid[0]
    id_player2 = list_of_pid[1]
    id_player3 = list_of_pid[2]
    id_player4 = list_of_pid[3]
    end_amount_player1 = 0.0
    end_amount_player2 = 0.0
    end_amount_player3 = 0.0
    end_amount_player4 = 0.0

    # Player 1
    result_player1 = Result(id_player1)
    result_player1.round1Sent = df.loc["Decision_in_Round1", result_player1.id]
    result_player1.round2Sent = df.loc["Decision_in_Round2", result_player1.id]
    result_player1.round3Sent = df.loc["Decision_in_Round3", result_player1.id]
    result_player1.round1Received = df.loc["Decision_in_Round1", id_player2]
    result_player1.round2Received = df.loc["Decision_in_Round2", id_player3]
    result_player1.round3Received = df.loc["Decision_in_Round3", id_player4]

    # Player 2
    result_player2 = Result(id_player2)
    result_player2.round1Sent = df.loc["Decision_in_Round1", result_player2.id]
    result_player2.round2Sent = df.loc["Decision_in_Round2", result_player2.id]
    result_player2.round3Sent = df.loc["Decision_in_Round3", result_player2.id]
    result_player2.round1Received = df.loc["Decision_in_Round1", id_player1]
    result_player2.round2Received = df.loc["Decision_in_Round2", id_player4]
    result_player2.round3Received = df.loc["Decision_in_Round3", id_player3]

    # Player 3
    result_player3 = Result(id_player3)
    result_player3.round1Sent = df.loc["Decision_in_Round1", result_player3.id]
    result_player3.round2Sent = df.loc["Decision_in_Round2", result_player3.id]
    result_player3.round3Sent = df.loc["Decision_in_Round3", result_player3.id]
    result_player3.round1Received = df.loc["Decision_in_Round1", id_player4]
    result_player3.round2Received = df.loc["Decision_in_Round2", id_player1]
    result_player3.round3Received = df.loc["Decision_in_Round3", id_player2]

    # Player 4
    result_player4 = Result(id_player4)
    result_player4.round1Sent = df.loc["Decision_in_Round1", result_player4.id]
    result_player4.round2Sent = df.loc["Decision_in_Round2", result_player4.id]
    result_player4.round3Sent = df.loc["Decision_in_Round3", result_player4.id]
    result_player4.round1Received = df.loc["Decision_in_Round1", id_player3]
    result_player4.round2Received = df.loc["Decision_in_Round2", id_player2]
    result_player4.round3Received = df.loc["Decision_in_Round3", id_player1]

    # Losen
    Entscheidung12_round1 = bool(random.getrandbits(1))
    Entscheidung34_round1 = bool(random.getrandbits(1))
    Entscheidung13_round2 = bool(random.getrandbits(1))
    Entscheidung24_round2 = bool(random.getrandbits(1))
    Entscheidung14_round3 = bool(random.getrandbits(1))
    Entscheidung23_round3 = bool(random.getrandbits(1))

    # 1 vs 2 round1
    print(Entscheidung12_round1)
    if Entscheidung12_round1:
        end_amount_player1 = end_amount_player1 + (10 - int(result_player1.round1Sent))
        end_amount_player2 = end_amount_player2 + int(result_player1.round1Sent)
        result_player1.round1MyDecision = True
        result_player2.round1MyDecision = False
    else:
        end_amount_player1 = end_amount_player1 + int(result_player2.round1Sent)
        end_amount_player2 = end_amount_player2 + (10 - int(result_player2.round1Sent))
        result_player1.round1MyDecision = False
        result_player2.round1MyDecision = True

    # 3 vs 4 round1
    if Entscheidung34_round1:
        end_amount_player3 = end_amount_player3 + (10 - int(result_player3.round1Sent))
        end_amount_player4 = end_amount_player4 + int(result_player3.round1Sent)
        result_player3.round1MyDecision = True
        result_player4.round1MyDecision = False
    else:
        end_amount_player3 = end_amount_player3 + int(result_player4.round1Sent)
        end_amount_player4 = end_amount_player4 + (10 - int(result_player4.round1Sent))
        result_player3.round1MyDecision = False
        result_player4.round1MyDecision = True

    # 1 vs 3 round2
    if Entscheidung13_round2:
        end_amount_player1 = end_amount_player1 + (10 - int(result_player1.round2Sent))
        end_amount_player3 = end_amount_player3 + int(result_player1.round2Sent)
        result_player1.round2MyDecision = True
        result_player3.round2MyDecision = False
    else:
        end_amount_player1 = end_amount_player1 + int(result_player3.round2Sent)
        end_amount_player3 = end_amount_player3 + (10 - int(result_player3.round2Sent))
        result_player1.round2MyDecision = False
        result_player3.round2MyDecision = True

    # 2 vs 4 round2
    if Entscheidung24_round2:
        end_amount_player2 = end_amount_player2 + (10 - int(result_player2.round2Sent))
        end_amount_player4 = end_amount_player4 + int(result_player2.round2Sent)
        result_player2.round2MyDecision = True
        result_player4.round2MyDecision = False
    else:
        end_amount_player2 = end_amount_player2 + int(result_player4.round2Sent)
        end_amount_player4 = end_amount_player4 + (10 - int(result_player4.round2Sent))
        result_player2.round2MyDecision = False
        result_player4.round2MyDecision = True

    # 1 vs 4 round3
    if Entscheidung14_round3:
        end_amount_player1 = end_amount_player1 + (10 - int(result_player1.round3Sent))
        end_amount_player4 = end_amount_player4 + int(result_player1.round3Sent)
        result_player1.round3MyDecision = True
        result_player4.round3MyDecision = False
    else:
        end_amount_player1 = end_amount_player1 + int(result_player4.round3Sent)
        end_amount_player4 = end_amount_player4 + (10 - int(result_player4.round3Sent))
        result_player1.round3MyDecision = False
        result_player4.round3MyDecision = True

    # 2 vs 3 round3
    if Entscheidung23_round3:
        end_amount_player2 = end_amount_player2 + (10 - int(result_player2.round3Sent))
        end_amount_player3 = end_amount_player3 + int(result_player2.round3Sent)
        result_player2.round3MyDecision = True
        result_player3.round3MyDecision = False
    else:
        end_amount_player2 = end_amount_player2 + int(result_player3.round3Sent)
        end_amount_player3 = end_amount_player3 + (10 - int(result_player3.round3Sent))
        result_player2.round3MyDecision = False
        result_player3.round3MyDecision = True

    df.loc["PayOutMoney in Euro", result_player1.id] = end_amount_player1 / 10
    df.loc["PayOutMoney in Euro", result_player2.id] = end_amount_player2 / 10
    df.loc["PayOutMoney in Euro", result_player3.id] = end_amount_player3 / 10
    df.loc["PayOutMoney in Euro", result_player4.id] = end_amount_player4 / 10

    global results
    results_list.append(result_player1)
    results_list.append(result_player2)
    results_list.append(result_player3)
    results_list.append(result_player4)


if __name__ == '__main__':
    app.run(host="0.0.0.0")
