import requests, json, time

getData = lambda api_url: json.loads(requests.get(api_url).content)

stations = getData('http://api.gios.gov.pl/pjp-api/rest/station/findAll')
stations = filter(lambda x: x['city']['name'] == 'Wrocław', stations)
stations = map(lambda x: { 'id': x['id'], 'stationName': x['stationName'] }, stations)
stations = list(stations)

while True:
    try:
        for s in stations:
            id = s['id']
            stationName = s['stationName'] 
            data = getData('http://api.gios.gov.pl/pjp-api/rest/aqindex/getIndex/{}'.format(id))
            stIndexLevel, indexLevelName, stSourceDataDate = data['stIndexLevel']['id'], data['stIndexLevel']['indexLevelName'], data['stSourceDataDate']
            print('{} [{}] {} - {} [{}]'.format(stationName, id, stSourceDataDate, indexLevelName, stIndexLevel))
    except:
        pass

    time.sleep(2)