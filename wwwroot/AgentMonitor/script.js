document.addEventListener('DOMContentLoaded', function () {
  loadAgents()
})

function loadAgents () {
  var includeDeleted = document.querySelector('#includeDeleted').checked

  var apiUrl = '/api/agent'
  var deletedApiUrl = '/api/agent/deleted'

  if (includeDeleted) {
    Promise.all([
      fetch(apiUrl + '?includeConnectionInformation=true').then(response =>
        response.json()
      ),
      fetch(deletedApiUrl + '?includeConnectionInformation=true').then(
        response => response.json()
      )
    ])
      .then(([agents, deletedAgents]) => {
        populateTable([...agents, ...deletedAgents])
      })
      .catch(error => console.error('Error fetching data:', error))
  } else {
    fetch(apiUrl + '?includeConnectionInformation=true')
      .then(response => response.json())
      .then(data => {
        populateTable(data)
      })
      .catch(error => console.error('Error fetching data:', error))
  }
}

function populateTable (agents) {
  var tbody = document.querySelector('#agentTable tbody')

  tbody.innerHTML = ''

  agents.forEach(agent => {
    var row = tbody.insertRow()

    addCell(row, agent.uuid)
    addCell(row, getStatusText(agent.status))
    addCell(row, agent.lastPing)
    addCell(row, agent.connectionInformation.address)
    addCell(row, agent.connectionInformation.port)
  })
}

function addCell (row, data) {
  var cell = row.insertCell()
  cell.textContent = data
}

function getStatusText (status) {
  switch (status) {
    case -2:
      return 'Deleted'
    case -1:
      return 'TroubleShooting'
    case 0:
      return 'Disconnected'
    case 1:
      return 'Connected'
    default:
      return 'Unknown'
  }
}
