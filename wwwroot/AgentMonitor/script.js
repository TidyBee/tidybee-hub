document.addEventListener('DOMContentLoaded', function () {
  fetch('/api/agent?includeConnectionInformation=true')
    .then(response => response.json())
    .then(data => {
      populateTable(data)
    })
    .catch(error => console.error('Error fetching data:', error))
})

function populateTable (agents) {
  var tbody = document.querySelector('#agentTable tbody')

  agents.forEach(agent => {
    console.log(agent)
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
