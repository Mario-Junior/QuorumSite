// Fetch and display legislators summary
async function loadLegislatorsSummary() {
    try {
        const response = await fetch('/api/summary/legislators');
        if (!response.ok) throw new Error('Failed to fetch legislators data');
        
        const data = await response.json();
        
        // Create chart
        const ctx = document.getElementById('legislatorsChart').getContext('2d');
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.map(l => l.legislator),
                datasets: [
                    {
                        label: 'Supported Bills',
                        data: data.map(l => l.supportedBills),
                        backgroundColor: '#7259ef',
                        borderColor: '#7259ef',
                        borderWidth: 1
                    },
                    {
                        label: 'Opposed Bills',
                        data: data.map(l => l.opposedBills),
                        backgroundColor: 'rgba(244, 67, 54, 0.8)',
                        borderColor: 'rgba(244, 67, 54, 1)',
                        borderWidth: 1
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Legislators Voting Activity'
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                }
            }
        });
        
        // Create table
        const tableHtml = `
            <table>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Legislator</th>
                        <th>Supported Bills</th>
                        <th>Opposed Bills</th>
                    </tr>
                </thead>
                <tbody>
                    ${data.map(l => `
                        <tr>
                            <td>${l.id}</td>
                            <td>${l.legislator}</td>
                            <td>${l.supportedBills}</td>
                            <td>${l.opposedBills}</td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;
        
        document.getElementById('legislators-table').innerHTML = tableHtml;
        
    } catch (error) {
        showError('Error loading legislators data: ' + error.message);
    }
}

// Fetch and display bills summary
async function loadBillsSummary() {
    try {
        const response = await fetch('/api/summary/bills');
        if (!response.ok) throw new Error('Failed to fetch bills data');
        
        const data = await response.json();
        
        // Create chart
        const ctx = document.getElementById('billsChart').getContext('2d');
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.map(b => b.bill.substring(0, 30) + '...'),
                datasets: [
                    {
                        label: 'Supporters',
                        data: data.map(b => b.supporters),
                        backgroundColor: 'rgba(76, 175, 80, 0.8)',
                        borderColor: 'rgba(76, 175, 80, 1)',
                        borderWidth: 1
                    },
                    {
                        label: 'Opposers',
                        data: data.map(b => b.opposers),
                        backgroundColor: 'rgba(255, 152, 0, 0.8)',
                        borderColor: 'rgba(255, 152, 0, 1)',
                        borderWidth: 1
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: 'Bills Voting Results'
                    }
                },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: {
                            stepSize: 1
                        }
                    }
                }
            }
        });
        
        // Create table
        const tableHtml = `
            <table>
                <thead>
                    <tr>
                        <th>ID</th>
                        <th>Bill</th>
                        <th>Supporters</th>
                        <th>Opposers</th>
                        <th>Primary Sponsor</th>
                    </tr>
                </thead>
                <tbody>
                    ${data.map(b => `
                        <tr>
                            <td>${b.id}</td>
                            <td>${b.bill}</td>
                            <td>${b.supporters}</td>
                            <td>${b.opposers}</td>
                            <td>${b.primarySponsor}</td>
                        </tr>
                    `).join('')}
                </tbody>
            </table>
        `;
        
        document.getElementById('bills-table').innerHTML = tableHtml;
        
    } catch (error) {
        showError('Error loading bills data: ' + error.message);
    }
}

function showError(message) {
    const errorContainer = document.getElementById('error-container');
    errorContainer.innerHTML = `<div class="error">${message}</div>`;
}

// Load all data on page load
window.addEventListener('DOMContentLoaded', () => {
    loadLegislatorsSummary();
    loadBillsSummary();
});
