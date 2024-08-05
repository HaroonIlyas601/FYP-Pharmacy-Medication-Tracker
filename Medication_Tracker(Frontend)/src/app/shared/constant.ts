

export const breadcrumbsMenu=[
    {
        label:'Categories',
        path:'/categories',
        children:[
            {
                path:':category'
            },
            {
                path:'/product/:id'
            }
        ]
    }
];

export const MENU:{
    title:string;
    path:string;
}[]
=[
    {
        title: 'Home',
        path: '/'
    },
    {
        title: 'Medicine',
        path: '/categories/Medicine'
    },
    
]

