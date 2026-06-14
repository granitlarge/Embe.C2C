import localFont from 'next/font/local';

export const calSans = localFont({
    src: [
        {
            path: './CalSans-Regular.ttf',
            weight: '400',
            style: 'normal',
        }
    ],
    variable: '--font-cal-sans',
    display: 'swap',
});

export const latoRegular = localFont({
    src: [
        {
            path: './Lato-Regular.ttf',
            weight: '400',
            style: 'normal',
        }
    ],
    variable: '--font-lato-regular',
    display: 'swap',
});