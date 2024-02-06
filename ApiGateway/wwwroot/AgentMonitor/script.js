document.addEventListener('DOMContentLoaded', function () {
  loadAgents()
})

var headers = document.querySelectorAll('#agentTable th')
headers.forEach(header => {
  header.addEventListener('click', function () {
    sortTable(this.cellIndex)
  })
})

function loadAgents () {
  var includeDeleted = document.querySelector('#includeDeleted').checked

  var apiUrl = '/gateway/auth/agent'
  var deletedApiUrl = '/gateway/auth/agent/deleted'

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

  var rows = document.querySelectorAll('#agentTable tbody tr')
  rows.forEach(row => {
    row.addEventListener('click', function () {
      var agentId = row.cells[0].textContent
      fetchAgentMetadata(agentId)
    })

    var statusCell = row.cells[1]
    if (statusCell.textContent.trim() !== 'Deleted') {
      addActionButton(row, 'Ping', function () {
        var agentId = row.cells[0].textContent
        pingAgent(agentId)
      })
      addActionButton(row, 'Disconnect', function () {
        var agentId = row.cells[0].textContent
        disconnectAgent(agentId)
      })

      addActionButton(row, 'Delete', function () {
        var agentId = row.cells[0].textContent
        deleteAgent(agentId)
      })
    } else {
      addActionButton(row, 'Restore', function () {
        var agentId = row.cells[0].textContent
        restoreAgent(agentId)
      })
    }
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

function sortTable (columnIndex) {
  var table, rows, switching, i, x, y, shouldSwitch
  table = document.getElementById('agentTable')
  switching = true

  while (switching) {
    switching = false
    rows = table.rows

    for (i = 1; i < rows.length - 1; i++) {
      shouldSwitch = false
      x = rows[i].getElementsByTagName('td')[columnIndex]
      y = rows[i + 1].getElementsByTagName('td')[columnIndex]

      if (columnIndex === 2) {
        shouldSwitch = new Date(x.textContent) < new Date(y.textContent)
      } else {
        shouldSwitch = x.textContent.toLowerCase() > y.textContent.toLowerCase()
      }

      if (shouldSwitch) {
        rows[i].parentNode.insertBefore(rows[i + 1], rows[i])
        switching = true
      }
    }
  }
}

function fetchAgentMetadata (agentId) {
  fetch(`/gateway/auth/agent/${agentId}?includeMetadata=true`)
    .then(response => response.json())
    .then(agent => {
      displayMetadata(agent.metadata.json)
    })
    .catch(error =>
      displayMetadata(
        'Metadata not available. This agent is currently deleted. You can restore it to retrieve the data.'
      )
    )
}

function displayMetadata (metadata) {
  var metadataContainer = document.getElementById('metadataContainer')
  metadataContainer.innerHTML = JSON.stringify(metadata, null, 2)
}

function addActionButton (row, label, clickHandler) {
  var cell = row.insertCell()
  var button = document.createElement('button')
  button.textContent = label
  button.addEventListener('click', clickHandler)
  cell.appendChild(button)
}

function pingAgent (agentId) {
  fetch(`/gateway/auth/aoth/${agentId}/ping`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({})
  })
    .then(response => {
      if (response.ok) {
        loadAgents()
      }
    })
    .catch(error => console.error('Error during ping:', error))
}

function disconnectAgent (agentId) {
  /// TODO Replace with sending to adgent disconnection request by querying AOTH api
  fetch(`/gateway/auth/aoth/${agentId}/disconnect`, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json'
    }
  })
    .then(response => {
      if (response.ok) {
        loadAgents()
      }
    })
    .catch(error => console.error('Error during disconnect:', error))
}

function deleteAgent (agentId) {
  /// TODO Replace with sending to adgent deletion request by querying AOTH api
  fetch(`/gateway/auth/aoth/${agentId}`, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json'
    }
  })
    .then(response => {
      loadAgents()
    })
    .catch(error => console.error('Error during delete:', error))
}

function restoreAgent (agentId) {
  /// TODO Restoring agent by query AOTH api
  console.log(`Restoring agent ${agentId}`)
}
