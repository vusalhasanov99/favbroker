// Get all anchor tags with the specified data attributes
const anchorTags = document.querySelectorAll('a[data-bId][data-vId]');

// Add a click event listener to each anchor tag
anchorTags.forEach(tag => {
    tag.addEventListener('click', function (event) {
        event.preventDefault(); // Prevent the default link behavior (e.g., navigating to a new page)

        // Get the data attributes from the clicked anchor tag
        const brandId = tag.getAttribute('data-bId');
        const visitId = tag.getAttribute('data-vId');

        // Construct the URL with route parameters using the data attributes
        const apiUrl = `https://dashboard.cybergagency.com/clickint/?brandId=${brandId}&visitId=${visitId}`;

        // Make a GET request to the API with the constructed URL
        fetch(apiUrl)
            .then(response => {
                
            })
            .catch(error => {

            });

        // Open the constructed URL in a new tab
        window.open(tag.getAttribute('href'), '_blank');
    });
});

