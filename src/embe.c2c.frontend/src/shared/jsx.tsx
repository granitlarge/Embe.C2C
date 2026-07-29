export function join(elements: (React.ReactNode | undefined)[], separator: (React.ReactNode)) {
    const ret = [];
    for (let i = 0; i < elements.length; i++) {
        ret.push(elements[i]);
        if (i !== elements.length - 1)
            ret.push(separator);
    }
    return (
        <>
            {ret}
        </>
    );
}